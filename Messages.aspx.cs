using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace FoodSaver
{
    public partial class Messages : Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        private bool IsSeller => (Session["SellerAuthenticated"] as bool?) == true && Session["SellerId"] != null;
        private int SellerId => Convert.ToInt32(Session["SellerId"]);
        private string SellerStoreName => (Session["SellerStoreName"] ?? "Seller").ToString();

        private int ConversationId
        {
            get
            {
                int cid;
                return int.TryParse(Request.QueryString["cid"], out cid) ? cid : -1;
            }
        }

        // Dynamic master: seller -> SellPage, otherwise customer -> Site
        protected void Page_PreInit(object sender, EventArgs e)
        {
            // If you renamed your masters, update these paths:
            if (IsSeller)
                MasterPageFile = "~/SellPage.master";
            else
                MasterPageFile = "~/Site.master";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblInfo.Text = "";

            if (IsSeller)
            {
                pnlSeller.Visible = true;
                pnlCustomerPlaceholder.Visible = false;

                if (!IsPostBack)
                {
                    LoadUsersDropdown();
                    LoadInbox();

                    if (ConversationId > 0 && SellerOwnsConversation(ConversationId))
                    {
                        OpenConversation(ConversationId);
                    }
                    else
                    {
                        pnlChat.Visible = false;
                        pnlNoChat.Visible = true;
                    }
                }
            }
            else
            {
                // Customer placeholder for now
                pnlSeller.Visible = false;
                pnlCustomerPlaceholder.Visible = true;
            }
        }

        // ====== INBOX ======
        private void LoadInbox()
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT
    c.ConversationID,
    u.FullName AS CustomerName,
    (
        SELECT COUNT(*)
        FROM Messages m
        WHERE m.ConversationID = c.ConversationID
          AND m.IsRead = 0
          AND NOT (m.SenderType = 'Seller' AND m.SenderID = @SellerId)
    ) AS UnreadCount
FROM Conversations c
JOIN Users u ON u.UserID = c.UserID
WHERE c.SellerID = @SellerId
ORDER BY ISNULL(c.LastMessageAt, c.CreatedAt) DESC;
", conn))
            {
                cmd.Parameters.AddWithValue("@SellerId", SellerId);

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                gvInbox.DataSource = dt;
                gvInbox.DataBind();
            }
        }

        protected void gvInbox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "OpenChat")
            {
                Response.Redirect("Messages.aspx?cid=" + e.CommandArgument);
            }
        }

        // ====== Start new chat (demo using Users table) ======
        private void LoadUsersDropdown()
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT UserID, FullName, Email
FROM Users
ORDER BY FullName;
", conn))
            {
                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                ddlUsers.Items.Clear();
                ddlUsers.Items.Add(new ListItem("-- Select a customer --", ""));

                foreach (DataRow row in dt.Rows)
                {
                    string text = $"{row["FullName"]} ({row["Email"]})";
                    ddlUsers.Items.Add(new ListItem(text, row["UserID"].ToString()));
                }
            }
        }

        protected void btnStartChat_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ddlUsers.SelectedValue))
            {
                lblError.Text = "Please select a customer.";
                return;
            }

            int userId = Convert.ToInt32(ddlUsers.SelectedValue);
            int cid = GetOrCreateConversation(userId, SellerId);
            Response.Redirect("Messages.aspx?cid=" + cid);
        }

        private int GetOrCreateConversation(int userId, int sellerId)
        {
            using (var conn = new SqlConnection(ConnStr))
            {
                conn.Open();

                // Check existing
                using (var check = new SqlCommand(@"
SELECT ConversationID
FROM Conversations
WHERE UserID = @UserID AND SellerID = @SellerID;
", conn))
                {
                    check.Parameters.AddWithValue("@UserID", userId);
                    check.Parameters.AddWithValue("@SellerID", sellerId);

                    object existing = check.ExecuteScalar();
                    if (existing != null) return Convert.ToInt32(existing);
                }

                // Create
                using (var create = new SqlCommand(@"
INSERT INTO Conversations (UserID, SellerID)
VALUES (@UserID, @SellerID);
SELECT SCOPE_IDENTITY();
", conn))

                {
                    create.Parameters.AddWithValue("@UserID", userId);
                    create.Parameters.AddWithValue("@SellerID", sellerId);

                    return Convert.ToInt32(create.ExecuteScalar());
                }
            }
        }

        // ====== CHAT ======
        private bool SellerOwnsConversation(int conversationId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT COUNT(*)
FROM Conversations
WHERE ConversationID = @Cid AND SellerID = @SellerId;
", conn))
            {
                cmd.Parameters.AddWithValue("@Cid", conversationId);
                cmd.Parameters.AddWithValue("@SellerId", SellerId);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        private void OpenConversation(int conversationId)
        {
            // Mark all customer messages as read
            MarkRead(conversationId);

            // Load header label “Chat with ___”
            lblChatWith.Text = " • " + GetCustomerName(conversationId);

            // Load messages
            LoadMessages(conversationId);

            pnlChat.Visible = true;
            pnlNoChat.Visible = false;

            // Refresh inbox unread counts
            LoadInbox();
        }

        private string GetCustomerName(int conversationId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT u.FullName
FROM Conversations c
JOIN Users u ON u.UserID = c.UserID
WHERE c.ConversationID = @Cid;
", conn))
            {
                cmd.Parameters.AddWithValue("@Cid", conversationId);
                conn.Open();
                return (cmd.ExecuteScalar() ?? "Customer").ToString();
            }
        }

        private void MarkRead(int conversationId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
UPDATE Messages
SET IsRead = 1
WHERE ConversationID = @Cid
  AND IsRead = 0
  AND SenderType = 'User';
", conn))
            {
                cmd.Parameters.AddWithValue("@Cid", conversationId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        private void LoadMessages(int conversationId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
SELECT
    m.MessageID,
    m.MessageText,
    m.SentAt,
    CASE WHEN m.SenderType='Seller' AND m.SenderID=@SellerId THEN 1 ELSE 0 END AS IsMine
FROM Messages m
WHERE m.ConversationID = @Cid
ORDER BY m.SentAt ASC;
", conn))
            {
                cmd.Parameters.AddWithValue("@Cid", conversationId);
                cmd.Parameters.AddWithValue("@SellerId", SellerId);

                conn.Open();
                var dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                rptMessages.DataSource = dt;
                rptMessages.DataBind();
            }
        }


        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (ConversationId <= 0 || !SellerOwnsConversation(ConversationId))
            {
                lblError.Text = "Invalid conversation.";
                return;
            }

            string text = (txtMessage.Text ?? "").Trim();
            if (text.Length == 0) { lblError.Text = "Message cannot be empty."; return; }
            if (text.Length > 1000) { lblError.Text = "Max 1000 characters."; return; }

            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
INSERT INTO Messages (ConversationID, SenderType, SenderID, MessageText, SentAt, IsRead)
VALUES (@Cid, 'Seller', @SellerId, @Text, GETDATE(), 0);

UPDATE Conversations
SET LastMessageAt = GETDATE()
WHERE ConversationID = @Cid;
", conn))
            {
                cmd.Parameters.AddWithValue("@Cid", ConversationId);
                cmd.Parameters.AddWithValue("@SellerId", SellerId);
                cmd.Parameters.AddWithValue("@Text", text);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtMessage.Text = "";
            OpenConversation(ConversationId);
        }

        protected void btnCtxDelete_Click(object sender, EventArgs e)
        {
            if (ConversationId <= 0 || !SellerOwnsConversation(ConversationId)) return;

            if (!int.TryParse(hfSelectedMessageId.Value, out int messageId)) return;

            bool deleted = DeleteOwnMessage(messageId, ConversationId, SellerId);
            if (!deleted) lblError.Text = "You can only delete your own messages.";

            OpenConversation(ConversationId);
        }

        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            if (ConversationId <= 0 || !SellerOwnsConversation(ConversationId)) return;

            if (!int.TryParse(hfSelectedMessageId.Value, out int messageId)) return;

            string newText = (hfEditText.Value ?? "").Trim();
            if (newText.Length == 0) { lblError.Text = "Message cannot be empty."; return; }
            if (newText.Length > 1000) { lblError.Text = "Max 1000 characters."; return; }

            bool updated = UpdateOwnMessage(messageId, ConversationId, SellerId, newText);
            if (!updated) lblError.Text = "You can only edit your own messages.";

            OpenConversation(ConversationId);
        }

        private bool UpdateOwnMessage(int messageId, int conversationId, int sellerId, string newText)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
UPDATE Messages
SET MessageText = @Text
WHERE MessageID = @Mid
  AND ConversationID = @Cid
  AND SenderType = 'Seller'
  AND SenderID = @SellerId;
SELECT @@ROWCOUNT;
", conn))
            {
                cmd.Parameters.AddWithValue("@Text", newText);
                cmd.Parameters.AddWithValue("@Mid", messageId);
                cmd.Parameters.AddWithValue("@Cid", conversationId);
                cmd.Parameters.AddWithValue("@SellerId", sellerId);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private bool DeleteOwnMessage(int messageId, int conversationId, int sellerId)
        {
            using (var conn = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(@"
DELETE FROM Messages
WHERE MessageID = @Mid
  AND ConversationID = @Cid
  AND SenderType = 'Seller'
  AND SenderID = @SellerId;
SELECT @@ROWCOUNT;
", conn))
            {
                cmd.Parameters.AddWithValue("@Mid", messageId);
                cmd.Parameters.AddWithValue("@Cid", conversationId);
                cmd.Parameters.AddWithValue("@SellerId", sellerId);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}

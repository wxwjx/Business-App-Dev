using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class SellerMessages : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDB"].ConnectionString;

        private int CurrentSellerId => Convert.ToInt32(Session["SellerID"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (Session["SellerID"] == null)
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }
            if (!IsPostBack)
            {
                BindInbox();

                if (int.TryParse(Request.QueryString["cid"], out int cid))
                {
                    OpenConversation(cid);
                }
            }
        }

        // ===== Inbox (Seller sees customers) =====
        private void BindInbox()
        {
            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                SELECT
                    c.ConversationID,
                    c.LastMessageAt,
                    u.FullName,
                    u.Email,
                    ISNULL((
                        SELECT TOP 1 m.MessageText
                        FROM Messages m
                        WHERE m.ConversationID = c.ConversationID
                        ORDER BY m.SentAt DESC
                    ), '') AS LastPreview,
                    ISNULL((
                        SELECT COUNT(*)
                        FROM Messages m2
                        WHERE m2.ConversationID = c.ConversationID
                          AND m2.SenderType = 'User'
                          AND m2.IsRead = 0
                    ), 0) AS UnreadCount
                FROM Conversations c
                INNER JOIN Users u ON u.UserID = c.UserID
                WHERE c.SellerID = @SellerID
                ORDER BY c.LastMessageAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@SellerID", CurrentSellerId);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    var p = (r["LastPreview"] ?? "").ToString();
                    r["LastPreview"] = p.Length > 60 ? p.Substring(0, 60) + "…" : p;
                }

                rptInbox.DataSource = dt;
                rptInbox.DataBind();
            }
        }

        protected void rptInbox_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            if (e.CommandName != "Open") return;

            hfEditingMessageID.Value = "";
            int cid = Convert.ToInt32(e.CommandArgument);
            OpenConversation(cid);
        }

        private void OpenConversation(int conversationId)
        {
            // Safety: only allow opening a conversation that belongs to this seller
            if (!SellerOwnsConversation(conversationId))
            {
                lblError.Text = "You can't open this conversation.";
                return;
            }

            hfConversationID.Value = conversationId.ToString();
            lblChatHeader.Text = "Conversation #" + conversationId;

            BindThread(conversationId);
            MarkOtherSideMessagesAsRead(conversationId, currentSide: "Seller");
            BindInbox();
        }

        private bool SellerOwnsConversation(int conversationId)
        {
            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Conversations
                WHERE ConversationID=@CID AND SellerID=@SID;", conn))
            {
                cmd.Parameters.AddWithValue("@CID", conversationId);
                cmd.Parameters.AddWithValue("@SID", CurrentSellerId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        // ===== Thread =====
        private void BindThread(int conversationId)
        {
            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                SELECT MessageID, SenderType, SenderID, MessageText, SentAt
                FROM Messages
                WHERE ConversationID = @ConversationID
                ORDER BY SentAt ASC;", conn))
            {
                cmd.Parameters.AddWithValue("@ConversationID", conversationId);

                var dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                dt.Columns.Add("IsMe", typeof(bool));
                foreach (DataRow r in dt.Rows)
                {
                    string senderType = r["SenderType"].ToString();
                    int senderId = Convert.ToInt32(r["SenderID"]);
                    r["IsMe"] = (senderType == "Seller" && senderId == CurrentSellerId);
                }

                rptMessages.DataSource = dt;
                rptMessages.DataBind();
            }
        }

        protected void rptMessages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var drv = (DataRowView)e.Item.DataItem;
            bool isMe = (bool)drv["IsMe"];
            int msgId = Convert.ToInt32(drv["MessageID"]);

            var pnlActions = (System.Web.UI.WebControls.Panel)e.Item.FindControl("pnlActions");
            var pnlView = (System.Web.UI.WebControls.Panel)e.Item.FindControl("pnlView");
            var pnlEdit = (System.Web.UI.WebControls.Panel)e.Item.FindControl("pnlEdit");

            pnlActions.Visible = isMe;

            bool editingThis = int.TryParse(hfEditingMessageID.Value, out int editingId) && editingId == msgId;
            pnlView.Visible = !editingThis;
            pnlEdit.Visible = editingThis;
        }

        protected void rptMessages_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(hfConversationID.Value, out int cid)) return;

            int msgId = Convert.ToInt32(e.CommandArgument);

            switch (e.CommandName)
            {
                case "Edit":
                    if (!CanModifyMessage(msgId)) return;
                    hfEditingMessageID.Value = msgId.ToString();
                    BindThread(cid);
                    break;

                case "Cancel":
                    hfEditingMessageID.Value = "";
                    BindThread(cid);
                    break;

                case "Save":
                    if (!CanModifyMessage(msgId)) return;
                    var txtEdit = (TextBox)e.Item.FindControl("txtEdit");
                    string newText = (txtEdit.Text ?? "").Trim();
                    if (newText.Length == 0) return;

                    UpdateMessageText(msgId, newText);
                    hfEditingMessageID.Value = "";
                    BindThread(cid);
                    BindInbox();
                    break;

                case "Delete":
                    if (!CanModifyMessage(msgId)) return;

                    UpdateMessageText(msgId, "This message was deleted");
                    hfEditingMessageID.Value = "";
                    BindThread(cid);
                    BindInbox();
                    break;
            }
        }

        private bool CanModifyMessage(int messageId)
        {
            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Messages m
                JOIN Conversations c ON c.ConversationID = m.ConversationID
                WHERE m.MessageID=@MessageID
                  AND m.SenderType='Seller'
                  AND m.SenderID=@SellerID
                  AND c.SellerID=@SellerID;", conn))
            {
                cmd.Parameters.AddWithValue("@MessageID", messageId);
                cmd.Parameters.AddWithValue("@SellerID", CurrentSellerId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void UpdateMessageText(int messageId, string newText)
        {
            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"UPDATE Messages SET MessageText=@T WHERE MessageID=@ID;", conn))
            {
                cmd.Parameters.AddWithValue("@T", newText);
                cmd.Parameters.AddWithValue("@ID", messageId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ===== Send =====
        protected void btnSend_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(hfConversationID.Value, out int cid))
            {
                lblError.Text = "Please select a conversation first.";
                return;
            }

            string msg = (txtMessage.Text ?? "").Trim();
            if (msg.Length == 0) return;

            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                INSERT INTO Messages (ConversationID, SenderType, SenderID, MessageText)
                VALUES (@ConversationID, 'Seller', @SenderID, @MessageText);

                UPDATE Conversations
                SET LastMessageAt = GETDATE()
                WHERE ConversationID = @ConversationID;", conn))
            {
                cmd.Parameters.AddWithValue("@ConversationID", cid);
                cmd.Parameters.AddWithValue("@SenderID", CurrentSellerId);
                cmd.Parameters.AddWithValue("@MessageText", msg);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            txtMessage.Text = "";
            BindThread(cid);
            BindInbox();
        }

        private void MarkOtherSideMessagesAsRead(int conversationId, string currentSide)
        {
            string other = currentSide == "User" ? "Seller" : "User";

            using (var conn = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
                UPDATE Messages
                SET IsRead = 1
                WHERE ConversationID = @ConversationID
                  AND SenderType = @OtherSide
                  AND IsRead = 0;", conn))
            {
                cmd.Parameters.AddWithValue("@ConversationID", conversationId);
                cmd.Parameters.AddWithValue("@OtherSide", other);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

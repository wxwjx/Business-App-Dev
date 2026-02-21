using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Business_App_Dev
{
    public partial class Chatbot : System.Web.UI.Page
    {
        private string ConnStr =>
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Helps prevent page jump on postback
            this.MaintainScrollPositionOnPostBack = true;

            if (!IsPostBack)
            {
                string name =
                    (Session["Name"] ?? Session["FullName"] ?? Session["Username"] ?? Session["UserName"])?.ToString();

                if (string.IsNullOrWhiteSpace(name)) name = "there";

                litChat.Text = BotBubble($"Hi <b>{Server.HtmlEncode(name)}</b>! 👋 How can I help you?");
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string userMsg = (txtMsg.Text ?? "").Trim();
            if (string.IsNullOrEmpty(userMsg)) return;

            litChat.Text += UserBubble(userMsg);

            bool isEscalation = IsEscalationMessage(userMsg);

            string reply = GetBotReply(userMsg);
            litChat.Text += BotBubble(reply);

            // Only log if NOT already logged inside EscalateToAdmin
            if (!isEscalation)
            {
                LogChat(userMsg, reply);
            }

            AppendLatestAdminReplyIfAny();

            txtMsg.Text = "";
            ClientScript.RegisterStartupScript(this.GetType(), "scrollChat", "scrollChatToBottom();", true);
        }

        // Suggested buttons handler
        protected void Quick_Click(object sender, EventArgs e)
        {
            var btn = sender as System.Web.UI.WebControls.Button;
            if (btn == null) return;

            txtMsg.Text = btn.CommandArgument;
            btnSend_Click(sender, e);

            // Auto-scroll chat to bottom
            ClientScript.RegisterStartupScript(this.GetType(), "scrollChat", "scrollChatToBottom();", true);
        }

        // ================= INTENT LOGIC =================
        private string GetBotReply(string msg)
        {
            string lower = (msg ?? "").ToLower();

            if (lower.Contains("latest order"))
                return ShowLatestOrder();

            if (lower.Contains("order"))
                return ShowAllOrders();

            if (lower.Contains("feedback") && lower.Contains("show"))
                return ShowMyFeedback();

            if (lower.Contains("give feedback") || (lower.Contains("feedback") && lower.Contains("give")))
                return "📝 <b>Want to leave feedback?</b><br/><br/>" +
                       "<a href='Feedback.aspx' class='chatLinkBtn'>👉 Click here to leave feedback</a>";

            if (lower.Contains("edit feedback") || lower.Contains("delete feedback"))
                return "To edit or delete feedback, please visit: <a href='Feedback.aspx'>Manage Feedback</a>";

            if (lower.Contains("about"))
                return AboutEcoEats();

            if (lower.Contains("chat history"))
                return ShowChatHistory();

            // 🔥 ESCALATION TRIGGER
            if (lower.Contains("admin") || lower.Contains("support") || lower.Contains("enquir"))
            {
                EscalateToAdmin(msg);
                return "💬 <b>Ok!</b> I’ve sent your enquiry to Admin Support.<br/>" +
                       "They will reply here once they respond ✅";
            }

            return "🤔 I'm not sure how to help with that.<br/><br/>" +
                   "If you need human help, type <b>admin support</b>.";
        }

        // ================= ORDERS =================
        private string ShowAllOrders()
        {
            if (Session["UserID"] == null)
                return "Please log in to view your orders.";

            int userId = Convert.ToInt32(Session["UserID"]);

            StringBuilder sb = new StringBuilder();
            sb.Append("<b>Your past orders:</b><br/>");

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT OrderID, TotalAmount, PayStatus, CreatedAt
                FROM [dbo].[Orders]
                WHERE UserID = @uid
                ORDER BY CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return "You have no orders yet.";

                    while (reader.Read())
                    {
                        sb.Append(
                            $"• Order #{reader["OrderID"]} | " +
                            $"${Convert.ToDecimal(reader["TotalAmount"]):F2} | " +
                            $"{reader["PayStatus"]} | " +
                            $"{Convert.ToDateTime(reader["CreatedAt"]).ToShortDateString()}<br/>"
                        );
                    }
                }
            }
            return sb.ToString();
        }

        private string ShowLatestOrder()
        {
            if (Session["UserID"] == null)
                return "Please log in to view your latest order.";

            int userId = Convert.ToInt32(Session["UserID"]);

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 OrderID, TotalAmount, PayStatus, CreatedAt
                FROM [dbo].[Orders]
                WHERE UserID = @uid
                ORDER BY CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return "You have no orders yet.";

                    reader.Read();

                    int orderId = Convert.ToInt32(reader["OrderID"]);
                    decimal total = Convert.ToDecimal(reader["TotalAmount"]);
                    string status = reader["PayStatus"].ToString();
                    DateTime dt = Convert.ToDateTime(reader["CreatedAt"]);

                    return $"<b>Your latest order:</b><br/>" +
                           $"Order #{orderId}<br/>" +
                           $"Amount: ${total:F2}<br/>" +
                           $"Status: {status}<br/>" +
                           $"Date: {dt.ToShortDateString()}";
                }
            }
        }

        // ================= FEEDBACK =================
        // NOTE: Requires a table named [dbo].[Feedback] with columns:
        // FeedbackID, UserID, Comments, Rating, CreatedAt
        private string ShowMyFeedback()
        {
            if (Session["UserID"] == null)
                return "Please log in to view your feedback.";

            int userId = Convert.ToInt32(Session["UserID"]);

            StringBuilder sb = new StringBuilder();
            sb.Append("<b>Your feedback:</b><br/>");

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT FeedbackID, Comments, Rating, CreatedAt
                FROM [dbo].[Feedback]
                WHERE UserID = @uid
                ORDER BY CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return "You have not submitted any feedback yet.";

                    while (reader.Read())
                    {
                        sb.Append(
                            $"• Rating: {reader["Rating"]}/5<br/>" +
                            $"Comment: {Server.HtmlEncode(reader["Comments"].ToString())}<br/>" +
                            $"Date: {Convert.ToDateTime(reader["CreatedAt"]).ToShortDateString()}<br/><br/>"
                        );
                    }
                }
            }

            sb.Append("Want to edit/delete or add new feedback? <a href='Feedback.aspx'>Manage Feedback</a>");
            return sb.ToString();
        }

        // ================= ABOUT =================
        private string AboutEcoEats()
        {
            return "🌿 EcoEats helps reduce food waste by connecting customers with surplus meals from sellers. " +
                   "You can order meals, track your past orders, and leave feedback to improve the platform.";
        }

        // ================= CHAT HISTORY =================
        private string ShowChatHistory()
        {
            if (Session["UserID"] == null)
                return "Please log in to view your chat history.";

            string userId = Session["UserID"].ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append("<b>Your recent chat history:</b><br/>");

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 8 UserMessage, BotReply, AdminReply, Status, CreatedAt
        FROM [dbo].[ChatbotLog]
        WHERE UserId = @uid
        ORDER BY CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@uid", userId);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return "No previous chat history found.";

                    while (reader.Read())
                    {
                        sb.Append($"• You: {Server.HtmlEncode(reader["UserMessage"].ToString())}<br/>");
                        sb.Append($"  Bot: {reader["BotReply"]}<br/>");

                        if (reader["AdminReply"] != DBNull.Value)
                        {
                            sb.Append($"  <b>Admin:</b> {Server.HtmlEncode(reader["AdminReply"].ToString())}<br/>");
                        }

                        sb.Append($"  <small>Status: {reader["Status"]} | {Convert.ToDateTime(reader["CreatedAt"]).ToShortDateString()}</small><br/><br/>");
                    }
                }
            }

            return sb.ToString();
        }

        // ================= LOGGING =================
        private int LogChat(string userMsg, string botReply, bool isEscalated = false, string status = "BOT")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnStr))
                using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO [dbo].[ChatbotLog] (UserId, UserMessage, BotReply, IsEscalated, Status)
            OUTPUT INSERTED.LogId
            VALUES (@uid, @msg, @reply, @esc, @status);", conn))
                {
                    object uid = DBNull.Value;
                    if (Session["UserID"] != null)
                        uid = Session["UserID"].ToString();

                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@msg", userMsg);
                    cmd.Parameters.AddWithValue("@reply", botReply);
                    cmd.Parameters.AddWithValue("@esc", isEscalated);
                    cmd.Parameters.AddWithValue("@status", status);

                    conn.Open();
                    int newId = Convert.ToInt32(cmd.ExecuteScalar());
                    return newId;
                }
            }
            catch
            {
                // chatbot still works even if logging fails
                return 0;
            }
        }

        // ================= UI HELPERS =================
        private string UserBubble(string msg)
        {
            return $"<div class='msg user'>{Server.HtmlEncode(msg)}</div>";
        }

        private string BotBubble(string msg)
        {
            return $"<div class='msg bot'>{msg}</div>";
        }
        private void EscalateToAdmin(string userMsg)
        {
            // must have user logged in
            if (Session["UserID"] == null) return;

            // 1) Log escalation and get the LogId
            int logId = LogChat(
                userMsg,
                "Escalated to Admin Support.",
                true,
                "OPEN"
            );

            // 2) Add notification to Admin bell
            // (Only if insert succeeded)
            if (logId > 0)
            {
                string who = (Session["Name"] ?? Session["FullName"] ?? "Customer").ToString();
                string shortMsg = userMsg ?? "";
                if (shortMsg.Length > 80) shortMsg = shortMsg.Substring(0, 80) + "...";

                NotificationHelper.Add(
                    "Chat",
                    "New Admin Enquiry",
                    $"{who}: {shortMsg}",
                    "chat",
                    logId
                );
            }
        }
        private bool IsEscalationMessage(string msg)
        {
            string lower = (msg ?? "").ToLower();
            return lower.Contains("admin") || lower.Contains("support") || lower.Contains("enquir");
        }
        private void AppendLatestAdminReplyIfAny()
        {
            if (Session["UserID"] == null) return;
            string uid = Session["UserID"].ToString();

            using (SqlConnection conn = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 AdminReply, CreatedAt
        FROM dbo.ChatbotLog
        WHERE UserId = @uid
          AND IsEscalated = 1
          AND AdminReply IS NOT NULL
        ORDER BY CreatedAt DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@uid", uid);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return;

                    string reply = r["AdminReply"].ToString();
                    DateTime dt = Convert.ToDateTime(r["CreatedAt"]);

                    litChat.Text += BotBubble(
                        $"✅ <b>Admin Support replied</b> ({dt:dd MMM yyyy HH:mm}):<br/>{Server.HtmlEncode(reply)}"
                    );
                }
            }
        }
    }
}

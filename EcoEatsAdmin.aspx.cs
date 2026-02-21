using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class EcoEatsAdmin : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserRole"] == null || Session["UserRole"].ToString() != "Admin")
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadPendingApplications();
                LoadFeedback();
                LoadChats();
            }
        }

        private void LoadPendingApplications()
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT Id, BusinessName, Owner, Email, Category, SubmitDate
                FROM SellerApplications
                WHERE Status = 'PENDING'
                ORDER BY SubmitDate DESC;", conn))
            {
                conn.Open();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    rptApps.DataSource = dt;
                    rptApps.DataBind();
                }
            }

            lblMsg.Text = "";
        }

        protected void rptApps_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!int.TryParse(e.CommandArgument.ToString(), out int appId))
                return;

            if (e.CommandName == "APPROVE")
            {
                ApproveSeller(appId);
                lblMsg.Text = "✅ Seller application approved.";
                ShowToast("Seller approved & email attempted");
                InsertNotification("Seller approved", $"Application #{appId} approved.");
            }
            else if (e.CommandName == "REJECT")
            {
                UpdateStatus(appId, "REJECTED");
                lblMsg.Text = "❌ Seller application rejected.";
                ShowToast("Application rejected", "error");
                InsertNotification("Seller rejected", $"Application #{appId} rejected.");
            }

            LoadPendingApplications();
        }

        private void ApproveSeller(int applicationId)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                // 1️⃣ Get application data (include phone)
                SqlCommand get = new SqlCommand(@"
                    SELECT BusinessName, Address, Email, PhoneNumber
                    FROM SellerApplications
                    WHERE Id = @Id
                ", conn);

                get.Parameters.AddWithValue("@Id", applicationId);

                string shopName, address, email, phone;

                using (var r = get.ExecuteReader())
                {
                    if (!r.Read())
                        return;

                    shopName = r["BusinessName"]?.ToString() ?? "";
                    address = r["Address"]?.ToString() ?? "";
                    email = r["Email"]?.ToString() ?? "";
                    phone = r["PhoneNumber"]?.ToString() ?? "";
                }

                // 2️⃣ Prevent duplicate seller insert
                SqlCommand check = new SqlCommand(@"
                    SELECT COUNT(1)
                    FROM Seller
                    WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))
                ", conn);

                check.Parameters.AddWithValue("@Email", email);

                if (Convert.ToInt32(check.ExecuteScalar()) > 0)
                    return;

                // 3️⃣ Insert into Seller table (include phone)
                SqlCommand insert = new SqlCommand(@"
                    INSERT INTO Seller (ShopName, Address, Email, Phone, CreatedAt)
                    VALUES (@ShopName, @Address, @Email, @Phone, GETDATE())
                ", conn);

                insert.Parameters.AddWithValue("@ShopName", shopName);
                insert.Parameters.AddWithValue("@Address", address);
                insert.Parameters.AddWithValue("@Email", email);
                insert.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);

                insert.ExecuteNonQuery();

                // 4️⃣ Update application status + send email
                UpdateStatus(applicationId, "APPROVED");

                // IMPORTANT: don't crash admin page if email fails
                try
                {
                    SendSellerApprovedEmail(email, shopName);
                }
                catch (SmtpException ex)
                {
                    // show a friendly message but keep approval successful
                    ShowToast("Approved, but email failed: " + SafeMsg(ex.Message), "warn");
                }
                catch (Exception ex)
                {
                    ShowToast("Approved, but email failed: " + SafeMsg(ex.Message), "warn");
                }
            }
        }

        private void UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE SellerApplications
                SET Status = @Status,
                    ApprovedAt = CASE WHEN @Status = 'APPROVED' THEN ISNULL(ApprovedAt, GETDATE()) ELSE ApprovedAt END,
                    RejectedAt = CASE WHEN @Status = 'REJECTED' THEN ISNULL(RejectedAt, GETDATE()) ELSE RejectedAt END
                WHERE Id = @Id;", conn))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SendSellerApprovedEmail(string toEmail, string shopName)
        {
            // Read the <smtp from="..."> value from Web.config
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string fromEmail = smtpSection?.From ?? "no-reply@ecoeats.local";

            string fromName = ConfigurationManager.AppSettings["EmailFromName"] ?? "EcoEats";

            var msg = new MailMessage();
            msg.From = new MailAddress(fromEmail, fromName);
            msg.To.Add(toEmail);

            msg.Subject = "EcoEats Seller Application Approved 🎉";
            msg.Body = $@"
Hello {shopName},

Great news! 🎉

Your seller application on EcoEats has been APPROVED.

You can now log in and start listing surplus food items on our platform.

Thank you for helping reduce food waste 🌱

Best regards,
EcoEats Team
";
            msg.IsBodyHtml = false;

            using (var smtp = CreateSmtpClient())
            {
                smtp.Send(msg);
            }
        }

        private SmtpClient CreateSmtpClient()
        {
            // Reads <system.net><mailSettings><smtp> from Web.config
            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            if (smtpSection == null || smtpSection.Network == null)
                throw new Exception("Missing <system.net><mailSettings> SMTP configuration in Web.config.");

            string host = smtpSection.Network.Host;
            int port = smtpSection.Network.Port;
            string user = smtpSection.Network.UserName ?? "";
            string pass = (smtpSection.Network.Password ?? "").Replace(" ", "");
            bool ssl = smtpSection.Network.EnableSsl;

            if (string.IsNullOrWhiteSpace(host))
                throw new Exception("SMTP host is missing in Web.config mailSettings.");

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                throw new Exception("SMTP username/password missing in Web.config mailSettings. Gmail requires an App Password.");

            var smtp = new SmtpClient(host, port)
            {
                EnableSsl = ssl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(user, pass)
            };

            return smtp;
        }

        private void LoadFeedback()
        {
            try
            {
                var selected = cblRatings.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value)
                    .ToList();

                using (SqlConnection conn = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    var sql = new StringBuilder(@"
                        SELECT 
                            f.FeedbackID,
                            f.UserID,
                            u.FullName AS CustomerName,
                            f.Rating,
                            f.Tag,
                            f.Comments,
                            f.CreatedAt
                        FROM Feedback f
                        INNER JOIN Users u ON f.UserID = u.UserID
                    ");

                    if (selected.Count > 0)
                    {
                        var placeholders = selected.Select((v, idx) => $"@r{idx}").ToArray();
                        sql.Append(" WHERE f.Rating IN (" + string.Join(",", placeholders) + ") ");

                        for (int i = 0; i < selected.Count; i++)
                            cmd.Parameters.AddWithValue($"@r{i}", int.Parse(selected[i]));
                    }

                    sql.Append(" ORDER BY f.CreatedAt DESC, f.FeedbackID DESC;");
                    cmd.CommandText = sql.ToString();

                    conn.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    rptFeedback.DataSource = dt;
                    rptFeedback.DataBind();

                    lblFeedbackMsg.Text = (dt.Rows.Count == 0) ? "No feedback matches your filter." : "";
                }
            }
            catch (Exception ex)
            {
                lblFeedbackMsg.Text = "Error loading feedback: " + ex.Message;
            }
        }

        public IHtmlString GetStars(int rating)
        {
            rating = Math.Max(0, Math.Min(5, rating));
            StringBuilder sb = new StringBuilder();

            for (int i = 1; i <= 5; i++)
            {
                sb.Append(i <= rating
                    ? "<span class='star filled'>★</span>"
                    : "<span class='star'>★</span>");
            }

            return new HtmlString(sb.ToString());
        }

        public string GetFeedbackPillClass(string tag)
        {
            switch ((tag ?? "").Trim().ToLower())
            {
                case "positive": return "positive";
                case "negative": return "negative";
                case "suggestion": return "suggestion";
                default: return "suggestion";
            }
        }

        protected void btnExportFeedback_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    u.FullName AS CustomerName,
                    f.Rating,
                    f.Tag,
                    f.Comments,
                    f.CreatedAt
                FROM Feedback f
                INNER JOIN Users u ON f.UserID = u.UserID
                ORDER BY f.CreatedAt DESC, f.FeedbackID DESC;", conn))
            {
                conn.Open();

                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("CustomerName,Rating,Tag,Comments,CreatedAt");

                foreach (DataRow row in dt.Rows)
                {
                    string name = row["CustomerName"].ToString().Replace("\"", "\"\"");
                    string tag = row["Tag"].ToString().Replace("\"", "\"\"");
                    string comments = row["Comments"].ToString().Replace("\"", "\"\"");
                    string rating = row["Rating"].ToString();
                    string date = Convert.ToDateTime(row["CreatedAt"]).ToString("yyyy-MM-dd HH:mm:ss");

                    csv.AppendLine($"\"{name}\",{rating},\"{tag}\",\"{comments}\",\"{date}\"");
                }

                Response.Clear();
                Response.ContentType = "text/csv";
                Response.AddHeader("Content-Disposition", "attachment;filename=EcoEatsFeedback.csv");
                Response.Write(csv.ToString());
                Response.End();
            }
        }

        private void LoadChats()
        {
            string filter = ddlChatStatus?.SelectedValue ?? "OPEN";

            string where = "";
            if (filter == "OPEN")
                where = "WHERE c.IsEscalated = 1 AND c.Status = 'OPEN'";
            else if (filter == "RESOLVED")
                where = "WHERE c.IsEscalated = 1 AND c.Status = 'RESOLVED'";
            else
                where = "WHERE c.IsEscalated = 1";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand($@"
        SELECT
            c.LogId,
            c.UserId,
            ISNULL(u.FullName, 'User') AS CustomerName,
            c.UserMessage AS IssueTitle,
            c.CreatedAt,
            c.Status
        FROM ChatbotLog c
        LEFT JOIN Users u ON u.UserID = TRY_CONVERT(int, c.UserId)
        {where}
        ORDER BY c.CreatedAt DESC;", conn))
            {
                conn.Open();
                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                rptChats.DataSource = dt;
                rptChats.DataBind();
            }
        }

        protected void rptChats_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "TAKEOVER")
            {
                ActiveReplyId = id;
                ((HiddenField)Master.FindControl("hfActiveTab")).Value = "chat";
                LoadChats();
            }
            else if (e.CommandName == "CANCEL")
            {
                ActiveReplyId = null;
                ((HiddenField)Master.FindControl("hfActiveTab")).Value = "chat";
                LoadChats();
            }
            else if (e.CommandName == "SEND")
            {
                var txt = (TextBox)e.Item.FindControl("txtReply");
                string reply = txt?.Text?.Trim();

                ((HiddenField)Master.FindControl("hfActiveTab")).Value = "chat";

                if (string.IsNullOrWhiteSpace(reply))
                {
                    ShowToast("Reply cannot be empty.", "warn");
                    return;
                }

                SaveReply(id, reply);
                ActiveReplyId = null;
                LoadChats();
                ShowToast("Reply sent!");
            }
        }

        private void ShowToast(string message, string type = "success")
        {
            string safe = message.Replace("\\", "\\\\").Replace("'", "\\'");
            ClientScript.RegisterStartupScript(
                GetType(),
                Guid.NewGuid().ToString(),
                $"showToast('{safe}','{type}');",
                true
            );
        }

        private static string SafeMsg(string msg)
        {
            msg = msg ?? "";
            if (msg.Length > 180) msg = msg.Substring(0, 180) + "...";
            return msg.Replace("\r", " ").Replace("\n", " ");
        }

        private void SaveReply(int id, string reply)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE dbo.ChatbotLog
        SET AdminReply = @reply,
            Status = 'RESOLVED'
        WHERE LogId = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@reply", reply);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public string GetInitial(object nameObj)
        {
            string s = Convert.ToString(nameObj) ?? "";
            s = s.Trim();
            return string.IsNullOrEmpty(s) ? "U" : s.Substring(0, 1).ToLower();
        }

        public bool IsReplying(object idObj)
        {
            if (ActiveReplyId == null) return false;
            return Convert.ToInt32(idObj) == ActiveReplyId.Value;
        }

        private int? ActiveReplyId
        {
            get => ViewState["ActiveReplyId"] as int?;
            set => ViewState["ActiveReplyId"] = value;
        }

        protected void ddlChatStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((HiddenField)Master.FindControl("hfActiveTab")).Value = "chat";
            LoadChats();
        }
        public bool HasTag(object tagObj)
        {
            return !string.IsNullOrWhiteSpace(Convert.ToString(tagObj));
        }

        public string GetRatingPillClass(int rating)
        {
            if (rating <= 2) return "bad";
            if (rating == 3) return "mid";
            return "good";
        }

        protected void btnApplyRatingFilter_Click(object sender, EventArgs e)
        {
            LoadFeedback();
        }

        protected void btnClearRatingFilter_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in cblRatings.Items)
                item.Selected = false;

            LoadFeedback();
        }

        private void InsertNotification(string title, string message)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO AdminNotifications (Type, Title, Message)
                VALUES ('System', @t, @m)", conn))
            {
                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@m", message);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
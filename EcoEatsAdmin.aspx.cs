using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;
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
                UpdateStatus(appId, "APPROVED");
                lblMsg.Text = "✅ Seller application approved.";
                ShowToast("Approve Granted");
            }
            else if (e.CommandName == "REJECT")
            {
                UpdateStatus(appId, "REJECTED");
                lblMsg.Text = "❌ Seller application rejected.";
                ShowToast("Application Rejected!", "error");
            }

            LoadPendingApplications();
        }

        private void UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE SellerApplications
                SET Status = @Status
                WHERE Id = @Id;", conn))
            {
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        private void LoadFeedback()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(@"
            SELECT FeedbackId, CustomerName, Rating, FeedbackType, FeedbackText, SubmitDate
            FROM CustomerFeedback
            ORDER BY SubmitDate DESC, FeedbackId DESC;", conn))
                {
                    conn.Open();
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());

                    rptFeedback.DataSource = dt;
                    rptFeedback.DataBind();

                    lblFeedbackMsg.Text = (dt.Rows.Count == 0) ? "No feedback yet." : "";
                }
            }
            catch (Exception ex)
            {
                lblFeedbackMsg.Text = "Error loading feedback: " + ex.Message;
            }
        }

        // ★★★★★ stars HTML
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

        // Map pill colors
        public string GetFeedbackPillClass(string type)
        {
            switch (type?.Trim().ToLower())
            {
                case "positive": return "positive";
                case "suggestion": return "suggestion";
                case "negative": return "negative";
                default: return "suggestion";
            }
        }

        // Optional: Export CSV
        protected void btnExportFeedback_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT CustomerName, Rating, FeedbackType, FeedbackText, SubmitDate
        FROM CustomerFeedback
        ORDER BY SubmitDate DESC, FeedbackId DESC;", conn))
            {
                conn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());

                StringBuilder csv = new StringBuilder();
                csv.AppendLine("CustomerName,Rating,FeedbackType,FeedbackText,SubmitDate");

                foreach (DataRow row in dt.Rows)
                {
                    string name = row["CustomerName"].ToString().Replace("\"", "\"\"");
                    string type = row["FeedbackType"].ToString().Replace("\"", "\"\"");
                    string text = row["FeedbackText"].ToString().Replace("\"", "\"\"");
                    string rating = row["Rating"].ToString();
                    string date = Convert.ToDateTime(row["SubmitDate"]).ToString("yyyy-MM-dd");

                    csv.AppendLine($"\"{name}\",{rating},\"{type}\",\"{text}\",\"{date}\"");
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
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT EscalationId, CustomerName, IssueTitle, Status, CreatedAt
        FROM ChatEscalations
        WHERE Status <> 'Resolved'
        ORDER BY 
            CASE WHEN Status = 'Urgent' THEN 1 ELSE 2 END,
            CreatedAt DESC", conn))
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
                ((HiddenField)Master.FindControl("hfActiveTab")).Value = "chat"; // stay on chat
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

                // stay on chat tab
                ((HiddenField)Master.FindControl("hfActiveTab")).Value = "chat";

                if (string.IsNullOrWhiteSpace(reply))
                {
                    ShowToast("Reply cannot be empty.", "warn");
                    return;
                }

                SaveReply(id, reply);      // ✅ updates DB + sets Status=Resolved
                ActiveReplyId = null;      // ✅ closes reply box
                LoadChats();               // ✅ refresh = chat disappears
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




        private void SaveReply(int id, string reply)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE ChatEscalations
        SET AdminReply = @reply,
            RepliedAt = GETDATE(),
            Status = 'Resolved'
        WHERE EscalationId = @id", conn))
            {
                cmd.Parameters.AddWithValue("@reply", reply);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private int? ActiveReplyId
        {
            get => ViewState["ActiveReplyId"] as int?;
            set => ViewState["ActiveReplyId"] = value;
        }

        public bool IsReplying(object idObj)
        {
            if (ActiveReplyId == null) return false;
            return Convert.ToInt32(idObj) == ActiveReplyId.Value;
        }

    }
}


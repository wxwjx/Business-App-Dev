using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI.WebControls;

namespace Business_App_Dev
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadKpis();
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {
            LoadBell();
        }
        private void LoadKpis()
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                // 1) Total Users
                int totalUsers = Convert.ToInt32(ExecScalar(conn,
                    "SELECT COUNT(*) FROM Users"));

                // 2) Total Sellers (from Seller table)
                int totalSellers = Convert.ToInt32(ExecScalar(conn,
                    "SELECT COUNT(*) FROM SellerApplications WHERE Status = 'APPROVED'"));

                // 3) Avg Rating + Total Reviews
                object avgObj = ExecScalar(conn,
                    "SELECT AVG(CAST(Rating AS FLOAT)) FROM Feedback");

                object countObj = ExecScalar(conn,
                    "SELECT COUNT(*) FROM Feedback");

                double avgRating = (avgObj == DBNull.Value || avgObj == null) ? 0.0 : Convert.ToDouble(avgObj);
                int reviewCount = (countObj == DBNull.Value || countObj == null) ? 0 : Convert.ToInt32(countObj);

                // 4) Total Chat Escalations (EDIT THIS WHERE CLAUSE based on your Messages table)
                // Option A (most common): a bit column IsEscalated = 1
                // int escalations = Convert.ToInt32(ExecScalar(conn,
                //     "SELECT COUNT(*) FROM Messages WHERE IsEscalated = 1"));

                // Option B: a status column EscalationStatus = 'Escalated'
                int escalations = Convert.ToInt32(ExecScalar(conn,
                    "SELECT COUNT(*) FROM ChatbotLog WHERE IsEscalated = 1 AND Status = 'OPEN'"));


                // Apply to UI (format)
                litTotalUsers.Text = totalUsers.ToString("N0");
                litTotalSellers.Text = totalSellers.ToString("N0");
                litAvgRating.Text = avgRating.ToString("0.0");
                litReviewCount.Text = reviewCount.ToString("N0");
                litChatEscalations.Text = escalations.ToString("N0");
            }
        }

        protected void kpiTimer_Tick(object sender, EventArgs e)
        {
            LoadKpis();
        }

        private object ExecScalar(SqlConnection conn, string sql)
        {
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                return cmd.ExecuteScalar();
            }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            // clear session cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var s = new HttpCookie("ASP.NET_SessionId", "");
                s.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(s);
            }

            // if you use FormsAuthentication anywhere, also clear it
            System.Web.Security.FormsAuthentication.SignOut();
            if (Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName] != null)
            {
                var auth = new HttpCookie(System.Web.Security.FormsAuthentication.FormsCookieName, "");
                auth.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(auth);
            }

            Response.Redirect("~/Login.aspx", true);
        }
        private void LoadBell()
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                // unread count
                SqlCommand c1 = new SqlCommand(
                    "SELECT COUNT(*) FROM AdminNotifications WHERE IsRead = 0", conn);
                int unread = (int)c1.ExecuteScalar();

                lblBellCount.Text = unread.ToString();
                lblBellCount.Visible = unread > 0;

                // list
                SqlCommand c2 = new SqlCommand(@"
            SELECT TOP 8 NotificationId, Title, Message, IsRead, CreatedAt
            FROM AdminNotifications WHERE IsRead = 0
            ORDER BY IsRead ASC, CreatedAt DESC", conn);

                DataTable dt = new DataTable();
                dt.Load(c2.ExecuteReader());

                rptBell.DataSource = dt;
                rptBell.DataBind();

                lblBellEmpty.Visible = dt.Rows.Count == 0;
            }
        }

        protected void rptBell_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "READ")
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(
                    "UPDATE AdminNotifications SET IsRead = 1 WHERE NotificationId=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", e.CommandArgument);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                LoadBell();
            }
        }

        protected void btnBellMarkAll_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(
                "UPDATE AdminNotifications SET IsRead = 1 WHERE IsRead = 0", conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            LoadBell();
        }

    }
}

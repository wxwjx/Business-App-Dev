using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;

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
                    "SELECT COUNT(*) FROM SellerApplications WHERE Status = 'Approved'"));

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
                    "SELECT COUNT(*) FROM ChatEscalations WHERE Status IN ('Pending', 'Urgent')"));


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


    }
}

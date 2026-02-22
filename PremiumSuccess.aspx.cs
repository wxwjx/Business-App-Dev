using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;
using Stripe;
using Stripe.Checkout;

namespace Business_App_Dev
{
    public partial class PremiumSuccess : Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            string sessionId = Request.QueryString["session_id"];
            if (string.IsNullOrEmpty(sessionId)) return;

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"];

            var service = new SessionService();
            var session = service.Get(sessionId);

            // ✅ Make sure Stripe says it is paid
            if (session.PaymentStatus != "paid") return;

            int userId = Convert.ToInt32(session.ClientReferenceId);

            UpgradeUserToPremium(userId);
        }

        private void UpgradeUserToPremium(int userId)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                string sql = @"
                    UPDATE Users
                    SET IsPremium = 1,
                        MemberSince = @Now
                    WHERE UserID = @UserID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Now", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
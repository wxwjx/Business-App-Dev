using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;

namespace Business_App_Dev
{
    public partial class Profile : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1) Must be logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // 2) Load profile details
                LoadProfile();
            }
        }

        private void LoadProfile()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserID"]);

                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = @"
                        SELECT UserID, FullName, Email, IsPremium, MemberSince
                        FROM Users
                        WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                Session.Clear();
                                Response.Redirect("Login.aspx");
                                return;
                            }

                            lblUserId.Text = reader["UserID"].ToString();
                            lblFullName.Text = reader["FullName"].ToString();
                            lblEmail.Text = reader["Email"].ToString();

                            bool isPremium = reader["IsPremium"] != DBNull.Value &&
                                             Convert.ToBoolean(reader["IsPremium"]);

                            if (isPremium)
                            {
                                lblMembershipTitle.Text = "Premium Member";
                                lblMembershipSubtitle.Text =
                                    "Thanks for supporting EcoEats! Enjoy exclusive perks and deeper discounts.";
                                lblStatus.Text = "Active";

                                if (reader["MemberSince"] != DBNull.Value)
                                {
                                    DateTime ms = Convert.ToDateTime(reader["MemberSince"]);
                                    lblMemberSince.Text = ms.ToString("dd MMM yyyy");
                                }
                                else
                                {
                                    lblMemberSince.Text = "-";
                                }

                                btnUpgrade.Visible = false;
                                lblMessage.Text = "You are currently a Premium member.";
                            }
                            else
                            {
                                lblMembershipTitle.Text = "EcoEats Member (Free)";
                                lblMembershipSubtitle.Text =
                                    "Upgrade to Premium to unlock exclusive deals and rewards.";
                                lblStatus.Text = "Free plan";
                                lblMemberSince.Text = "-";

                                btnUpgrade.Visible = true;
                                lblMessage.Text = "";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblMessage.Text = "Unable to load profile details. Please try again later.";
                btnUpgrade.Visible = false;
            }
        }

        // ✅ IMPORTANT: With Stripe, this button should ONLY redirect to checkout.
        // The DB update (IsPremium=1) must happen on PremiumSuccess.aspx AFTER payment success.
        protected void btnUpgrade_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            // Go to your Stripe checkout page
            Response.Redirect("PremiumCheckout.aspx");
        }

        protected void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            lblAccountActionMsg.Text = "";

            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    string sql = "DELETE FROM Users WHERE UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        int rows = cmd.ExecuteNonQuery();

                        if (rows == 0)
                        {
                            lblAccountActionMsg.Text = "Account not found or already deleted.";
                            return;
                        }
                    }
                }

                Session.Clear();
                Session.Abandon();

                lblAccountActionMsg.ForeColor = System.Drawing.Color.Green;
                lblAccountActionMsg.Text = "Account deleted successfully. Redirecting to login...";

                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            catch (Exception)
            {
                lblAccountActionMsg.Text = "Something went wrong while deleting your account.";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Sign out forms auth (if used)
            FormsAuthentication.SignOut();

            // Expire auth cookie
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var auth = new HttpCookie(FormsAuthentication.FormsCookieName, "");
                auth.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(auth);
            }

            // Expire session cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var s = new HttpCookie("ASP.NET_SessionId", "");
                s.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(s);
            }

            Response.Redirect("Login.aspx", true);
        }
    }
}
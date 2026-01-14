using System;
using System.Configuration;
using System.Data.SqlClient;

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
                // 1. User must be logged in
                if (Session["UserID"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // 2. Load details from DB
                LoadProfile();
            }
        }

        private void LoadProfile()
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
                        if (reader.Read())
                        {
                            // Basic info
                            lblUserId.Text = reader["UserID"].ToString();
                            lblFullName.Text = reader["FullName"].ToString();
                            lblEmail.Text = reader["Email"].ToString();

                            bool isPremium = reader["IsPremium"] != DBNull.Value &&
                                             Convert.ToBoolean(reader["IsPremium"]);

                            if (isPremium)
                            {
                                lblMembershipTitle.Text = "Premium Member";
                                lblMembershipSubtitle.Text = "Thanks for supporting EcoEats! Enjoy exclusive perks and deeper discounts.";
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

                                // Already premium → hide button
                                btnUpgrade.Visible = false;
                                lblMessage.Text = "You are currently a Premium member.";
                            }
                            else
                            {
                                lblMembershipTitle.Text = "EcoEats Member (Free)";
                                lblMembershipSubtitle.Text = "Upgrade to Premium to unlock exclusive deals and rewards.";
                                lblStatus.Text = "Free plan";
                                lblMemberSince.Text = "-";

                                btnUpgrade.Visible = true;
                                lblMessage.Text = "";
                            }
                        }
                        else
                        {
                            // Safety: if no user found, clear session
                            Session.Clear();
                            Response.Redirect("Login.aspx");
                        }
                    }
                }
            }
        }

        protected void btnUpgrade_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserID"]);

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

            lblMessage.Text = "Your membership has been upgraded to Premium!";
            LoadProfile();   // refresh labels
        }
    }
}

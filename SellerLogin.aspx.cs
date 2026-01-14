using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;

namespace FoodSaver
{
    public partial class SellerLogin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If already authenticated, go to dashboard
            if (Session["SellerAuthenticated"] as bool? == true)
            {
                Response.Redirect("SellerDashboard.aspx");
                return;
            }

            // Optional: auto-fill email from cookie
            if (!IsPostBack && Request.Cookies["SellerRemember"] != null)
            {
                txtEmail.Text = Request.Cookies["SellerRemember"].Value;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                SELECT Id, BusinessName, Email, PasswordHash, PasswordSalt, Status
                FROM SellerApplications
                WHERE Email = @Email
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                    {
                        ShowFail("Invalid email or password.");
                        return;
                    }

                    // Short-term gate: only allow Approved (your signup will insert Approved)
                    string status = r["Status"]?.ToString() ?? "Pending";
                    if (!status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowFail("Your seller account is not approved yet.");
                        return;
                    }

                    int sellerAppId = Convert.ToInt32(r["Id"]); // short-term "SellerId"
                    string storeName = r["BusinessName"]?.ToString() ?? "(Unknown Store)";

                    string dbHash = r["PasswordHash"]?.ToString() ?? "";
                    string dbSalt = r["PasswordSalt"]?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(dbHash) || string.IsNullOrWhiteSpace(dbSalt))
                    {
                        ShowFail("Account is missing password setup. Please sign up again.");
                        return;
                    }

                    if (!VerifyPassword(password, dbSalt, dbHash))
                    {
                        ShowFail("Invalid email or password.");
                        return;
                    }

                    // Success sessions (keep same keys so the rest of your seller pages keep working)
                    Session["SellerAuthenticated"] = true;
                    Session["SellerId"] = sellerAppId;
                    Session["SellerEmail"] = email;
                    Session["SellerStoreName"] = storeName;

                    // Remember-me cookie (optional)
                    if (chkRemember.Checked)
                    {
                        var cookie = new HttpCookie("SellerRemember", email)
                        {
                            Expires = DateTime.Now.AddDays(30)
                        };
                        Response.Cookies.Add(cookie);
                    }
                    else
                    {
                        // Clear cookie if exists
                        if (Request.Cookies["SellerRemember"] != null)
                        {
                            var cookie = new HttpCookie("SellerRemember")
                            {
                                Expires = DateTime.Now.AddDays(-1)
                            };
                            Response.Cookies.Add(cookie);
                        }
                    }

                    Response.Redirect("~/SellerDashboard.aspx");
                }
            }
        }

        private void ShowFail(string msg)
        {
            vsSummary.HeaderText = "Sign-in failed";
            vsSummary.Controls.Clear();
            vsSummary.Controls.Add(new System.Web.UI.LiteralControl(msg));
        }

        private static bool VerifyPassword(string password, string base64Salt, string expectedHash)
        {
            byte[] saltBytes = Convert.FromBase64String(base64Salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                string computed = Convert.ToBase64String(pbkdf2.GetBytes(32));
                return computed == expectedHash;
            }
        }
    }
}



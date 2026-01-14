using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Business_App_Dev
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (!IsPostBack)
            {
                if (Request.QueryString["err"] == "2fa_locked")
                    lblError.Text = "Too many invalid 2FA attempts. Please login again.";
                else if (Request.QueryString["err"] == "mfa_mismatch")
                    lblError.Text = "MFA account mismatch. Please try again.";
            }
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string email = (txtEmail.Text ?? "").Trim();
            string password = txtPassword.Text ?? "";

            // Always clear any old 2FA flow BEFORE starting a new login
            Session.Remove("Pending2FAEmail");
            Session.Remove("TwoFAAttempts");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter your email and password.";
                return;
            }

            string passwordHash = Sha256(password);

            // ✅ Admin password check
            if (!IsValidAdmin(email, passwordHash))
            {
                lblError.Text = "Invalid admin email or password.";
                return;
            }

            // ✅ Start 2FA flow
            Session["Pending2FAEmail"] = email;
            Session["TwoFAAttempts"] = 0;

            var twofa = GetAdmin2FA(email);
            if (!twofa.enabled || string.IsNullOrWhiteSpace(twofa.secret))
            {
                Response.Redirect("~/Enable2FA.aspx");
                return;
            }

            Response.Redirect("~/Verify2FA.aspx");
        }

        private bool IsValidAdmin(string email, string passwordHash)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Admin
                WHERE Email=@Email AND PasswordHash=@PasswordHash AND IsActive=1
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        private (bool enabled, string secret) GetAdmin2FA(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TwoFAEnabled, TwoFASecret
                FROM Admin
                WHERE Email = @Email
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return (false, null);

                    bool enabled = Convert.ToBoolean(r["TwoFAEnabled"]);
                    string secret = r["TwoFASecret"] as string;

                    return (enabled, secret);
                }
            }
        }

        private static string Sha256(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = sha.ComputeHash(bytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}

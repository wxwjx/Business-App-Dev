using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Business_App_Dev
{
    public partial class Login : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";

            // init attempts counter if missing


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
            string role = rblRole.SelectedValue;
            string email = (txtEmail.Text ?? "").Trim();
            string password = txtPassword.Text ?? "";

            // Always clear any old 2FA flow BEFORE starting a new login
            Session.Remove("Pending2FAEmail");
            Session.Remove("TwoFAAttempts");

            // ✅ reCAPTCHA check (ADD THIS HERE)
            if (!IsCaptchaValid())
            {
                lblError.Text = "❌ Please verify that you are not a robot.";
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter your email and password.";
                return;
            }

            string passwordHash = Sha256(password);

            if (role == "Admin")
            {
                // ✅ Admin password check (your existing Admin query)
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
                return;
            }
            // ✅ Customer login flow (NO 2FA)
            if (role == "Customer")
            {
                if (!TryLoginUser(email, password, out int userId))
                {
                    lblError.Text = "Invalid customer login.";
                    return;
                }

                Session["UserId"] = userId;
                Session["UserEmail"] = email;
                Session["UserRole"] = "Customer";
                Response.Redirect("Product.aspx");
                return;
            }

            // ✅ Seller login flow (NO 2FA)
            if (role == "Seller")
            {
                if (!TryLoginSeller(email, password, out int sellerId, out string status))
                {
                    // status can explain why
                    if (status == "Pending")
                        lblError.Text = "Your seller application is still pending approval.";
                    else if (status == "Rejected" || status == "REJECTED")
                        lblError.Text = "Your seller application was rejected.";
                    else
                        lblError.Text = "Invalid seller email or password.";
                    return;
                }

                Session["SellerId"] = sellerId;
                Session["UserEmail"] = email;
                Session["UserRole"] = "Seller";

                Response.Redirect("SellerDashboard.aspx");
                return;
            }



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
        //customer login method
        private bool TryLoginUser(string email, string password, out int userId)
        {
            userId = 0;

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT UserId, Password
        FROM Users
        WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))
    ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return false;

                    userId = Convert.ToInt32(r["UserId"]);
                    string stored = r["Password"]?.ToString() ?? "";

                    // ❌ Reject non-hashed passwords
                    if (!stored.StartsWith("pbkdf2$"))
                        return false;

                    return VerifyPbkdf2(password, stored);
                }
            }
        }
        private bool VerifyPbkdf2(string password, string stored)
        {
            // Format: pbkdf2$iterations$saltBase64$hashBase64
            var parts = stored.Split('$');
            if (parts.Length != 4) return false;
            if (parts[0] != "pbkdf2") return false;

            if (!int.TryParse(parts[1], out int iterations))
                return false;

            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] storedKey = Convert.FromBase64String(parts[3]);

            byte[] computedKey;
            using (var pbkdf2 = new Rfc2898DeriveBytes(
                password, salt, iterations, HashAlgorithmName.SHA256))
            {
                computedKey = pbkdf2.GetBytes(storedKey.Length);
            }

            return FixedTimeEquals(storedKey, computedKey);
        }
        private bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
        private bool TryLoginSeller(string email, string password, out int sellerId, out string status)
        {
            sellerId = 0;
            status = "";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 Id, PasswordHash, Status
        FROM SellerApplications
        WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))
    ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return false;

                    sellerId = Convert.ToInt32(r["Id"]);
                    status = (r["Status"]?.ToString() ?? "").Trim();
                    string stored = (r["PasswordHash"]?.ToString() ?? "").Trim();

                    // ❌ Not approved
                    if (!status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                        return false;

                    // ❌ Invalid hash
                    if (!stored.StartsWith("pbkdf2$"))
                        return false;

                    return VerifyPbkdf2(password, stored);
                }
            }
        }
        private bool IsCaptchaValid()
        {
            string secretKey = Environment.GetEnvironmentVariable("RECAPTCHA_SECRET");
            string response = Request.Form["g-recaptcha-response"];

            if (string.IsNullOrWhiteSpace(secretKey))
                return false;

            if (string.IsNullOrWhiteSpace(response))
                return false;

            string url = $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={response}";

            using (var client = new WebClient())
            {
                string result = client.DownloadString(url);
                return result.Contains("\"success\": true");
            }
        }

    }
}
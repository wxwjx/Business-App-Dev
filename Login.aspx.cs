using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Business_App_Dev
{
    public partial class Login : System.Web.UI.Page
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

        // -------------------------
        // NEW helpers: email/phone
        // -------------------------
        private bool LooksLikeEmail(string s)
        {
            s = (s ?? "").Trim();
            return s.Contains("@");
        }

        private string NormalizePhoneToE164(string input, string cc)
        {
            input = (input ?? "").Trim();

            // keep digits and '+'
            string cleaned = new string(input.Where(ch => char.IsDigit(ch) || ch == '+').ToArray());

            if (cleaned.StartsWith("+"))
            {
                // E.164 already
                cleaned = "+" + new string(cleaned.Skip(1).Where(char.IsDigit).ToArray());
                return cleaned;
            }

            // digits only -> prefix selected country code
            string digits = new string(cleaned.Where(char.IsDigit).ToArray());
            return (cc ?? "+65") + digits;
        }

        private bool IsValidE164(string phone)
        {
            // E.164: + followed by 8-15 digits
            return Regex.IsMatch(phone ?? "", @"^\+\d{8,15}$");
        }

        // -------------------------
        // Sign in
        // -------------------------
        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            string role = rblRole.SelectedValue;

            // NEW: login input (email OR phone)
            string loginId = (txtLoginId.Text ?? "").Trim();
            string password = txtPassword.Text ?? "";

            // Always clear any old 2FA flow BEFORE starting a new login
            Session.Remove("Pending2FAEmail");
            Session.Remove("TwoFAAttempts");

            // reCAPTCHA check
            if (!IsCaptchaValid())
            {
                lblError.Text = "❌ Please verify that you are not a robot.";
                return;
            }

            if (string.IsNullOrWhiteSpace(loginId) || string.IsNullOrWhiteSpace(password))
            {
                lblError.Text = "Please enter your email/phone and password.";
                return;
            }

            string email = null;
            string phone = null;

            if (role == "Admin")
            {
                // Admin must use email
                if (!LooksLikeEmail(loginId))
                {
                    lblError.Text = "Admin must login with email.";
                    return;
                }

                email = loginId;
                string passwordHash = Sha256(password);

                if (!IsValidAdmin(email, passwordHash))
                {
                    lblError.Text = "Invalid admin email or password.";
                    return;
                }

                // Start 2FA flow
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

            string mode = (hfLoginMode.Value ?? "Email").Trim(); // "Email" or "Phone"

            // Admin forced Email no matter what
            if (role == "Admin") mode = "Email";

            if (mode.Equals("Email", StringComparison.OrdinalIgnoreCase))
            {
                email = loginId;

                if (string.IsNullOrWhiteSpace(email) || !LooksLikeEmail(email))
                {
                    lblError.Text = "Please enter a valid email address.";
                    ClearLoginFields();
                    return;
                }
            }
            else
            {
                // Phone mode
                phone = NormalizePhoneToE164(loginId, ddlLoginCountryCode.SelectedValue);

                if (!IsValidE164(phone))
                {
                    lblError.Text = "Please enter a valid phone number.";
                    return;
                }
            }


            // Customer login
            if (role == "Customer")
            {
                if (!TryLoginUser(email, phone, password, out int userId, out string userEmail))
                {
                    lblError.Text = "Invalid customer login.";
                    ClearLoginFields();
                    return;
                }

                Session["UserId"] = userId;
                Session["UserEmail"] = userEmail;   // store real email from DB
                Session["UserRole"] = "Customer";
                Response.Redirect("Product.aspx");
                return;
            }

            // Seller login
            if (role == "Seller")
            {
                if (!TryLoginSeller(email, phone, password, out int sellerId, out string status, out string sellerEmail))
                {
                    if (status == "Pending")
                        lblError.Text = "Your seller application is still pending approval.";
                    else if (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                        lblError.Text = "Your seller application was rejected.";
                    else
                        lblError.Text = "Invalid seller login.";

                    ClearLoginFields();   // ✅ clear everything
                    return;
                }

                Session["SellerId"] = sellerId;
                Session["UserEmail"] = sellerEmail; // store real email
                Session["UserRole"] = "Seller";
                Response.Redirect("SellerDashboard.aspx");
                return;
            }
        }

        // -------------------------
        // Admin
        // -------------------------
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

        // -------------------------
        // Customer login (email OR phone)
        // -------------------------
        private bool TryLoginUser(string email, string phone, string password, out int userId, out string userEmail)
        {
            userId = 0;
            userEmail = "";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 UserId, Email, Password
                FROM Users
                WHERE
                    (@Email IS NOT NULL AND LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
                 OR (@Phone IS NOT NULL AND LTRIM(RTRIM(PhoneNumber)) = LTRIM(RTRIM(@Phone)))
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);

                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return false;

                    userId = Convert.ToInt32(r["UserId"]);
                    userEmail = (r["Email"]?.ToString() ?? "").Trim();
                    string stored = r["Password"]?.ToString() ?? "";

                    // Reject non-hashed passwords
                    if (!stored.StartsWith("pbkdf2$"))
                        return false;

                    return VerifyPbkdf2(password, stored);
                }
            }
        }

        // -------------------------
        // Seller login (email OR phone)
        // Requires SellerApplications.PhoneNumber column
        // -------------------------
        private bool TryLoginSeller(string email, string phone, string password, out int sellerId, out string status, out string sellerEmail)
        {
            sellerId = 0;
            status = "";
            sellerEmail = "";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 Id, Email, PasswordHash, Status
                FROM SellerApplications
                WHERE
                    (@Email IS NOT NULL AND LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
                 OR (@Phone IS NOT NULL AND LTRIM(RTRIM(PhoneNumber)) = LTRIM(RTRIM(@Phone)))
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", (object)phone ?? DBNull.Value);

                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read())
                        return false;

                    sellerId = Convert.ToInt32(r["Id"]);
                    sellerEmail = (r["Email"]?.ToString() ?? "").Trim();
                    status = (r["Status"]?.ToString() ?? "").Trim();
                    string stored = (r["PasswordHash"]?.ToString() ?? "").Trim();

                    // Not approved
                    if (!status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                        return false;

                    // Invalid hash
                    if (!stored.StartsWith("pbkdf2$"))
                        return false;

                    return VerifyPbkdf2(password, stored);
                }
            }
        }

        // -------------------------
        // PBKDF2 verify helpers
        // -------------------------
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

        // -------------------------
        // SHA for admin only
        // -------------------------
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

        // -------------------------
        // reCAPTCHA
        // -------------------------
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
        private void ClearLoginFields()
        {
            txtLoginId.Text = "";
            txtPassword.Text = "";
            ddlLoginCountryCode.SelectedIndex = 0;
            hfLoginMode.Value = "Email";
        }

    }
}

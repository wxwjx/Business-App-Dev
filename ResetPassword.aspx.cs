using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;

namespace Business_App_Dev
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMsg.Text = "";

            if (!IsPostBack)
            {
                if (!IsTokenValid(Request.QueryString["token"], out _))
                {
                    pnlReset.Visible = false;
                    pnlInvalid.Visible = true;
                    lblInvalid.Text = "This reset link is invalid or expired.";
                }
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            string token = Request.QueryString["token"];
            string pw = txtNewPassword.Text ?? "";
            string cf = txtConfirmPassword.Text ?? "";

            if (string.IsNullOrWhiteSpace(pw) || string.IsNullOrWhiteSpace(cf))
            {
                lblMsg.Text = "Please fill in both password fields.";
                return;
            }

            if (!IsStrongPassword(pw, out string msg))
            {
                lblMsg.Text = msg;
                return;
            }

            if (pw != cf)
            {
                lblMsg.Text = "Passwords do not match.";
                return;
            }

            if (!IsTokenValid(token, out string email))
            {
                pnlReset.Visible = false;
                pnlInvalid.Visible = true;
                lblInvalid.Text = "This reset link is invalid or expired.";
                return;
            }

            string hashed = HashPassword(pw);

            bool updated = false;

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();

                // 1) Try update Customer
                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE Users
                    SET Password = @Password
                    WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))
                ", conn))
                {
                    cmd.Parameters.AddWithValue("@Password", hashed);
                    cmd.Parameters.AddWithValue("@Email", email);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0) updated = true;
                }

                // 2) If not customer, try update Seller (approved seller OR pending seller)
                if (!updated)
                {
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE SellerApplications
                        SET PasswordHash = @Password
                        WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))
                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@Password", hashed);
                        cmd.Parameters.AddWithValue("@Email", email);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0) updated = true;
                    }
                }

                // 3) If neither updated, show error (safety)
                if (!updated)
                {
                    pnlReset.Visible = false;
                    pnlInvalid.Visible = true;
                    lblInvalid.Text = "Account not found. Please request reset again.";
                    return;
                }

                // 4) Mark token used (ONLY after successful update)
                using (SqlCommand cmd2 = new SqlCommand(@"
                    UPDATE PasswordResets
                    SET IsUsed = 1
                    WHERE Token = @Token
                ", conn))
                {
                    cmd2.Parameters.AddWithValue("@Token", token);
                    cmd2.ExecuteNonQuery();
                }
            }

            Response.Redirect("~/Login.aspx");
        }


        private bool IsTokenValid(string token, out string email)
        {
            email = null;
            token = (token ?? "").Trim();
            if (string.IsNullOrWhiteSpace(token)) return false;

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 Email, ExpiresAt, IsUsed
                FROM PasswordResets
                WHERE Token = @Token
                ORDER BY CreatedAt DESC
            ", conn))
            {
                cmd.Parameters.AddWithValue("@Token", token);
                conn.Open();

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return false;

                    bool used = Convert.ToBoolean(r["IsUsed"]);
                    DateTime expires = Convert.ToDateTime(r["ExpiresAt"]);
                    if (used) return false;
                    if (DateTime.Now > expires) return false;

                    email = r["Email"].ToString();
                    return true;
                }
            }
        }

        private bool IsStrongPassword(string password, out string message)
        {
            message = "";

            if (password.Length < 8)
            {
                message = "Password must be at least 8 characters.";
                return false;
            }

            bool hasLetter = password.Any(char.IsLetter);
            bool hasNumber = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (!hasLetter || !hasNumber || !hasSpecial)
            {
                message = "Password must contain at least 1 letter, 1 number, and 1 special character.";
                return false;
            }

            return true;
        }

        private string HashPassword(string password)
        {
            const int iterations = 100000;
            const int saltSize = 16;
            const int keySize = 32;

            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                key = pbkdf2.GetBytes(keySize);

            return $"pbkdf2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }
    }
}
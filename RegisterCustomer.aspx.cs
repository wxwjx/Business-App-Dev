using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Web.Script.Services;

namespace Business_App_Dev
{
    public partial class RegisterCustomer : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string fullName = (txtFullName.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string password = txtPassword.Text ?? "";
            string confirm = txtConfirm.Text ?? "";

            lblError.Text = "";

            // 1) Basic required checks
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                lblError.Text = "Please fill in all fields.";
                return;
            }

            // 2) Email format
            if (!IsValidEmail(email))
            {
                lblError.Text = "Please enter a valid email address.";
                return;
            }

            // 3) Terms checkbox
            if (!chkTerms.Checked)
            {
                lblError.Text = "Please agree to the Terms and Conditions.";
                return;
            }

            // 4) Password strength
            if (!IsStrongPassword(password, out string pwMsg))
            {
                lblError.Text = pwMsg;
                return;
            }

            // 5) Confirm password match
            if (password != confirm)
            {
                lblError.Text = "Password and Confirm Password do not match.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    // Check duplicate email
                    using (SqlCommand checkCmd = new SqlCommand(
                        "SELECT COUNT(1) FROM Users WHERE Email = @Email", conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            lblError.Text = "This email is already registered.";
                            return;
                        }
                    }

                    // ✅ Hash password (PBKDF2)
                    string hashedPassword = HashPassword(password);

                    // Insert new customer
                    using (SqlCommand insertCmd = new SqlCommand(@"
                        INSERT INTO Users (FullName, Email, Password, IsPremium, MemberSince)
                        VALUES (@FullName, @Email, @Password, @IsPremium, @MemberSince)
                    ", conn))
                    {
                        insertCmd.Parameters.AddWithValue("@FullName", fullName);
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@IsPremium", false);
                        insertCmd.Parameters.AddWithValue("@MemberSince", DateTime.Now);

                        insertCmd.ExecuteNonQuery();
                    }
                }

                Response.Redirect("~/Login.aspx");
            }
            catch
            {
                // Don't show detailed errors to users
                lblError.Text = "Something went wrong. Please try again.";
            }
        }

        // ------------------------
        // Validation Helpers
        // ------------------------
        private bool IsStrongPassword(string password, out string message)
        {
            message = "";

            if (string.IsNullOrWhiteSpace(password))
            {
                message = "Password cannot be empty.";
                return false;
            }

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

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        // ------------------------
        // Password Hashing (PBKDF2)
        // Stores: pbkdf2$<iterations>$<saltBase64>$<hashBase64>
        // ------------------------
        private string HashPassword(string password)
        {
            const int iterations = 100000;   // good baseline
            const int saltSize = 16;         // 128-bit
            const int keySize = 32;          // 256-bit

            byte[] salt = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                key = pbkdf2.GetBytes(keySize);
            }

            string saltB64 = Convert.ToBase64String(salt);
            string keyB64 = Convert.ToBase64String(key);

            return $"pbkdf2${iterations}${saltB64}${keyB64}";
        }

        // Use this later in Login.aspx.cs to verify:
        private bool VerifyPassword(string password, string stored)
        {
            // stored format: pbkdf2$iterations$salt$hash
            var parts = (stored ?? "").Split('$');
            if (parts.Length != 4) return false;
            if (parts[0] != "pbkdf2") return false;

            int iterations = int.Parse(parts[1]);
            byte[] salt = Convert.FromBase64String(parts[2]);
            byte[] storedKey = Convert.FromBase64String(parts[3]);

            byte[] computedKey;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                computedKey = pbkdf2.GetBytes(storedKey.Length);
            }

            return FixedTimeEquals(storedKey, computedKey);
        }

        private bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static int EmailExists(string email)
        {
            email = (email ?? "").Trim();
            if (string.IsNullOrWhiteSpace(email)) return 0;

            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0 ? 1 : 0;
                }
            }
        }
        protected void txtEmail_TextChanged(object sender, EventArgs e)
        {
            string email = (txtEmail.Text ?? "").Trim();

            lblEmailStatus.Text = "";

            if (!IsValidEmail(email))
            {
                lblEmailStatus.Text = "Please enter a valid email address.";
                lblEmailStatus.CssClass = "field-msg bad";
                return;
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email)))",
                    conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        lblEmailStatus.Text = "This email is already registered.";
                        lblEmailStatus.CssClass = "field-msg bad";
                    }
                    else
                    {
                        lblEmailStatus.Text = "Email is available ✅";
                        lblEmailStatus.CssClass = "field-msg ok";
                    }
                }
            }
        }



    }
}
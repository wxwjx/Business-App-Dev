using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text.RegularExpressions;



namespace Business_App_Dev
{
    public partial class RegisterSeller : System.Web.UI.Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string owner = (txtOwnerName.Text ?? "").Trim();
            string store = (txtStoreName.Text ?? "").Trim();
            string email = (txtEmail.Text ?? "").Trim();
            string address = (txtAddress.Text ?? "").Trim();
            string password = txtPassword.Text ?? "";
            string confirm = txtConfirm.Text ?? "";
            // ✅ Read selected food categories
            string categories = string.Join(", ",
                cblCategories.Items.Cast<ListItem>()
                    .Where(i => i.Selected)
                    .Select(i => i.Value));

            if (string.IsNullOrWhiteSpace(categories))
            {
                lblError.Text = "Please select at least one food category.";
                return;
            }



            // ---------- BASIC VALIDATION ----------
            if (string.IsNullOrWhiteSpace(owner) ||
                string.IsNullOrWhiteSpace(store) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                lblError.Text = "Please fill in all required fields.";
                return;
            }

            if (!IsValidEmail(email))
            {
                lblError.Text = "Please enter a valid email address.";
                return;
            }

            if (!chkTerms.Checked)
            {
                lblError.Text = "Please agree to the Terms and Conditions.";
                return;
            }

            if (!IsStrongPassword(password, out string pwMsg))
            {
                lblError.Text = pwMsg;
                return;
            }

            if (password != confirm)
            {
                lblError.Text = "Passwords do not match.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(_connStr))
                {
                    conn.Open();

                    using (SqlCommand dup = new SqlCommand(@"
                        SELECT
                            (SELECT COUNT(1)
                             FROM Users
                             WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
                          + (SELECT COUNT(1)
                             FROM SellerApplications
                             WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
                    ", conn))
                    {
                        dup.Parameters.AddWithValue("@Email", email.Trim());
                        int exists = Convert.ToInt32(dup.ExecuteScalar());

                        if (exists > 0)
                        {
                            lblError.Text = "This email is already registered as a customer or seller.";
                            return;
                        }
                    }


                    // ---------- HASH PASSWORD ----------
                    string hashed = HashPassword(password);

                    // ---------- INSERT SELLER APPLICATION ----------
                    using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO SellerApplications
                            (BusinessName, Owner, Email, Address, Category, Status, SubmitDate, PasswordHash)
                            VALUES
                            (@BusinessName, @Owner, @Email, @Address, @Category, @Status, @SubmitDate, @PasswordHash)

                    ", conn))
                    {
                        cmd.Parameters.AddWithValue("@BusinessName", store);
                        cmd.Parameters.AddWithValue("@Owner", owner);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Address", address);
                        cmd.Parameters.AddWithValue("@Category", categories);
                        cmd.Parameters.AddWithValue("@Status", "Pending");
                        cmd.Parameters.AddWithValue("@SubmitDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashed);

                        cmd.ExecuteNonQuery();
                    }
                }

                // ✅ REDIRECT TO WAITING PAGE
                Response.Redirect("~/SellerPending.aspx");
            }
            catch
            {
                lblError.Text = "Something went wrong. Please try again.";
            }
        }

        // =====================
        // HELPERS
        // =====================
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
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

        // PBKDF2 (same format as customer)
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

                using (SqlCommand cmd = new SqlCommand(@"
            SELECT
                (SELECT COUNT(1) FROM Users WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
              + (SELECT COUNT(1) FROM SellerApplications WHERE LOWER(LTRIM(RTRIM(Email))) = LOWER(LTRIM(RTRIM(@Email))))
        ", conn))
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

        protected void txtOwnerName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace FoodSaver
{
    public partial class SellerSignup : System.Web.UI.Page
    {
        protected void btnSignup_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            string storeName = txtStoreName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string address = txtAddress.Text.Trim();
            string postal = txtPostal.Text.Trim();
            string phone = txtPhone.Text.Trim();

            CreatePasswordHash(password, out string hash, out string salt);

            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
                INSERT INTO Sellers (StoreName, Email, PasswordHash, PasswordSalt, Address, PostalCode, Phone)
                VALUES (@StoreName, @Email, @Hash, @Salt, @Address, @Postal, @Phone);
            ", conn))
            {
                cmd.Parameters.AddWithValue("@StoreName", storeName);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Parameters.AddWithValue("@Salt", salt);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Postal", string.IsNullOrWhiteSpace(postal) ? (object)DBNull.Value : postal);
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Response.Redirect("~/SellerLogin.aspx");
                }
                catch (SqlException ex)
                {
                    // Duplicate email (UNIQUE constraint)
                    var msg = ex.Message.ToLower();
                    if (msg.Contains("unique") || msg.Contains("duplicate"))
                    {
                        vsSummarySignup.HeaderText = "Signup failed";
                        vsSummarySignup.Controls.Clear();
                        vsSummarySignup.Controls.Add(new System.Web.UI.LiteralControl("This email is already registered."));
                    }
                    else
                    {
                        vsSummarySignup.HeaderText = "Signup failed";
                        vsSummarySignup.Controls.Clear();
                        vsSummarySignup.Controls.Add(new System.Web.UI.LiteralControl("Signup failed. Please try again."));
                    }
                }
            }
        }

        private static void CreatePasswordHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                hash = Convert.ToBase64String(hashBytes);
                salt = Convert.ToBase64String(saltBytes);
            }
        }
    }
}

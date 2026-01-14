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

            string businessName = txtStoreName.Text.Trim();
            string owner = txtOwner.Text.Trim();
            string category = ddlCategory.SelectedValue;
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            CreatePasswordHash(password, out string hash, out string salt);

            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (var conn = new SqlConnection(cs))
            using (var cmd = new SqlCommand(@"
INSERT INTO SellerApplications
(BusinessName, Owner, Email, Category, Status, SubmitDate, PasswordHash, PasswordSalt)
VALUES
(@BusinessName, @Owner, @Email, @Category, @Status, GETDATE(), @Hash, @Salt);
", conn))
            {
                cmd.Parameters.AddWithValue("@BusinessName", businessName);
                cmd.Parameters.AddWithValue("@Owner", owner);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Category", category);

                // Short-term: skip approval => auto-approve
                cmd.Parameters.AddWithValue("@Status", "Approved");

                cmd.Parameters.AddWithValue("@Hash", hash);
                cmd.Parameters.AddWithValue("@Salt", salt);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    Response.Redirect("~/SellerLogin.aspx");
                }
                catch (SqlException ex)
                {
                    // If you added UNIQUE index on Email, this catches duplicates
                    string msg = ex.Message.ToLower();
                    ShowFail(msg.Contains("unique") || msg.Contains("duplicate")
                        ? "This email is already registered."
                        : "Signup failed. Please try again.");
                }
            }
        }

        private void ShowFail(string message)
        {
            vsSummarySignup.HeaderText = "Signup failed";
            vsSummarySignup.Controls.Clear();
            vsSummarySignup.Controls.Add(new System.Web.UI.LiteralControl(message));
        }

        private static void CreatePasswordHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                hash = Convert.ToBase64String(pbkdf2.GetBytes(32));
                salt = Convert.ToBase64String(saltBytes);
            }
        }
    }
}

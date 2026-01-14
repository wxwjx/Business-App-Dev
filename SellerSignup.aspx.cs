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
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // 1) SellerApplications (AUTH)
                        using (var cmdApp = new SqlCommand(@"
INSERT INTO SellerApplications
(BusinessName, Owner, Email, Category, Status, SubmitDate, PasswordHash, PasswordSalt)
VALUES
(@BusinessName, @Owner, @Email, @Category, 'Approved', GETDATE(), @Hash, @Salt);
", conn, tx))
                        {
                            cmdApp.Parameters.AddWithValue("@BusinessName", businessName);
                            cmdApp.Parameters.AddWithValue("@Owner", owner);
                            cmdApp.Parameters.AddWithValue("@Email", email);
                            cmdApp.Parameters.AddWithValue("@Category", category);
                            cmdApp.Parameters.AddWithValue("@Hash", hash);
                            cmdApp.Parameters.AddWithValue("@Salt", salt);

                            cmdApp.ExecuteNonQuery();
                        }

                        // 2) Ensure Seller profile exists (needed because Conversations FK uses dbo.Seller)
                        using (var cmdSeller = new SqlCommand(@"
IF NOT EXISTS (SELECT 1 FROM Seller WHERE Email = @Email)
BEGIN
    INSERT INTO Seller (ShopName, Address, PostalCode, Latitude, Longitude, PickupWindow, CreatedAt, Email)
    VALUES (@ShopName, 'Address not provided (temporary)', NULL, NULL, NULL, NULL, GETDATE(), @Email);
END
", conn, tx))
                        {
                            cmdSeller.Parameters.AddWithValue("@ShopName", businessName);
                            cmdSeller.Parameters.AddWithValue("@Email", email);
                            cmdSeller.ExecuteNonQuery();
                        }


                        tx.Commit();
                        Response.Redirect("~/SellerLogin.aspx");
                    }
                    catch (SqlException ex)
                    {
                        tx.Rollback();

                        string msg = ex.Message.ToLower();
                        ShowFail(msg.Contains("unique") || msg.Contains("duplicate")
                            ? "This email is already registered."
                            : "Signup failed. Please try again.");
                    }
                    catch
                    {
                        tx.Rollback();
                        ShowFail("Signup failed. Please try again.");
                    }
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

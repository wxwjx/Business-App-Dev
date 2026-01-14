using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Web.UI;

namespace FoodSaver
{
    public partial class SellerSignup : Page
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

            // extra guard (in case dropdown validation somehow bypassed)
            if (string.IsNullOrWhiteSpace(category))
            {
                ShowFail("Category is required.");
                return;
            }

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
    INSERT INTO Seller
        (ShopName, Address, PostalCode, Latitude, Longitude, PickupWindow, CreatedAt, Email)
    VALUES
        (@ShopName, 'Address not provided (temporary)', NULL, NULL, NULL, NULL, GETDATE(), @Email);
END
", conn, tx))
                        {
                            cmdSeller.Parameters.AddWithValue("@ShopName", businessName);
                            cmdSeller.Parameters.AddWithValue("@Email", email);

                            cmdSeller.ExecuteNonQuery();
                        }

                        // Commit once, only here
                        tx.Commit();

                        // Redirect safely (no ThreadAbortException)
                        Response.Redirect("~/SellerLogin.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    }
                    catch (SqlException ex)
                    {
                        // Rollback can throw if tx is already dead/completed -> wrap it
                        try { tx.Rollback(); } catch { /* ignore rollback errors */ }

                        string msg = (ex.Message ?? "").ToLowerInvariant();

                        // common unique constraint patterns
                        bool duplicate =
                            msg.Contains("unique") ||
                            msg.Contains("duplicate") ||
                            msg.Contains("cannot insert duplicate key") ||
                            msg.Contains("violation of unique key constraint");

                        ShowFail(duplicate
                            ? "This email is already registered."
                            : "Signup failed. Please try again.");

                        return;
                    }
                    catch (Exception)
                    {
                        try { tx.Rollback(); } catch { /* ignore rollback errors */ }

                        ShowFail("Signup failed. Please try again.");
                        return;
                    }
                }
            }
        }

        private void ShowFail(string message)
        {
            // ValidationSummary is meant to show validator errors, but this works as a simple message box too.
            // If you want it styled, ensure vsSummarySignup CssClass exists.
            vsSummarySignup.HeaderText = "Signup failed";
            vsSummarySignup.Controls.Clear();
            vsSummarySignup.Controls.Add(new System.Web.UI.LiteralControl(message));
        }

        private static void CreatePasswordHash(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256))
            {
                hash = Convert.ToBase64String(pbkdf2.GetBytes(32));
                salt = Convert.ToBase64String(saltBytes);
            }
        }
    }
}

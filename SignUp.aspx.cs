using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;


namespace Business_App_Dev
{
    public partial class SignUp : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSignUp_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirm = txtConfirm.Text.Trim();

            // 1. Basic validation
            if (string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirm))
            {
                lblSignUpMessage.Text = "Please fill in all fields.";
                return;
            }

            if (password.Length < 6)
            {
                lblSignUpMessage.Text = "Password must be at least 6 characters long.";
                return;
            }

            if (password != confirm)
            {
                lblSignUpMessage.Text = "Passwords do not match.";
                return;
            }

            // 2. Connect to database
            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // 2a. Check if email already exists
                string checkSql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, con))
                {
                    checkCmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        lblSignUpMessage.Text = "An account with this email already exists.";
                        return;
                    }
                }

                // 2b. Insert new record
                string insertSql = "INSERT INTO Users (FullName, Email, Password) VALUES (@FullName, @Email, @Password)";
                using (SqlCommand cmd = new SqlCommand(insertSql, con))
                {
                    cmd.Parameters.AddWithValue("@FullName", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // plain text ONLY for school assignment

                    cmd.ExecuteNonQuery();
                }
            }

            // 3. Save to session (for Profile later)
            Session["UserName"] = name;
            Session["UserEmail"] = email;

            // 4. Redirect to Login
            Response.Redirect("Login.aspx");
        }
    }
}

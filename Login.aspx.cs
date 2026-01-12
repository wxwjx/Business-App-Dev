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
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string email = txtLoginEmail.Text.Trim();
            string password = txtLoginPassword.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                lblLoginMessage.Text = "Please enter both email and password.";
                return;
            }

            string cs = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;
            string sql = @"SELECT UserID, FullName, IsPremium
                   FROM Users
                   WHERE Email = @Email AND Password = @Password";

            using (SqlConnection con = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Get values from DB
                        int userId = Convert.ToInt32(reader["UserID"]);
                        string fullName = reader["FullName"].ToString();
                        bool isPremium = reader["IsPremium"] != DBNull.Value &&
                                         Convert.ToBoolean(reader["IsPremium"]);

                        // Store in session
                        Session["UserID"] = userId;
                        Session["FullName"] = fullName;
                        Session["Email"] = email;
                        Session["IsPremium"] = isPremium;

                        // After login go to Home (Product page)
                        Response.Redirect("Product.aspx");
                    }
                    else
                    {
                        lblLoginMessage.Text = "Invalid email or password.";
                    }
                }
            }
        }

    }
}


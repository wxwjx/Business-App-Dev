using System;
using System.Web;
using System.Web.UI;

namespace FoodSaver
{
    public partial class SellerLogin : Page
    {
        // Demo credentials - replace with real validation against a DB or identity provider
        private const string DemoEmail = "seller@example.com";
        private const string DemoPassword = "Password123";

        protected void Page_Load(object sender, EventArgs e)
        {
            // If already authenticated, go to dashboard
            if (Session["SellerAuthenticated"] as bool? == true)
            {
                Response.Redirect("SellerDashboard.aspx");
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;

            // Replace this block with proper authentication logic (hashing, DB lookup, etc.)
            if (string.Equals(email, DemoEmail, StringComparison.OrdinalIgnoreCase)
                && password == DemoPassword)
            {
                Session["SellerAuthenticated"] = true;
                Session["SellerEmail"] = email;

                // Optionally persist a simple cookie when "Remember me" is checked (demo only)
                if (chkRemember.Checked)
                {
                    var cookie = new HttpCookie("SellerRemember", email) { Expires = DateTime.Now.AddDays(30) };
                    Response.Cookies.Add(cookie);
                }

                Response.Redirect("SellerDashboard.aspx");
                return;
            }

            // Show generic error without exposing specifics
            vsSummary.HeaderText = "Sign-in failed";
            vsSummary.Controls.Add(new LiteralControl("Invalid email or password."));
        }
    }
}
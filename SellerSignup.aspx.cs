using System;
using System.Web;
using System.Web.UI;

namespace FoodSaver
{
    public partial class SellerSignup : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If already authenticated, jump to dashboard
            if (Session["SellerAuthenticated"] as bool? == true)
            {
                Response.Redirect("SellerDashboard.aspx");
            }
        }

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid) return;

            string storeName = txtStoreName.Text.Trim();
            string email = txtEmailSignup.Text.Trim();
            string password = txtPasswordSignup.Text; // In production, never store plain text

            // Demo placeholder: create a simple session entry to mark seller as authenticated.
            // Replace with DB-backed registration and secure password hashing in production.
            Session["SellerAuthenticated"] = true;
            Session["SellerEmail"] = email;
            Session["SellerStoreName"] = storeName;

            // Optionally set a cookie for persistent login (demo only)
            var remember = new HttpCookie("SellerSignedUp", email) { Expires = DateTime.Now.AddDays(30) };
            Response.Cookies.Add(remember);

            // Go to seller dashboard after signup
            Response.Redirect("SellerDashboard.aspx");
        }
    }
}
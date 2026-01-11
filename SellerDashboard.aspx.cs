using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FoodSaver
{
    public partial class SellerDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["SellerAuthenticated"] as bool? != true)
            {
                Response.Redirect("~/SellerLogin.aspx");
                return;
            }

        }
    }
}

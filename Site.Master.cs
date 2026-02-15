using System;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LANG"] == null)
                Session["LANG"] = "en";

            if (!IsPostBack)
            {
                string lang = Session["LANG"].ToString();
                var item = ddlLanguage.Items.FindByValue(lang);
                if (item != null)
                {
                    ddlLanguage.ClearSelection();
                    item.Selected = true;
                }

                // Optional: highlight active tab automatically
                string path = (Request.Url.AbsolutePath ?? "").ToLowerInvariant();

                if (path.EndsWith("/product") || path.EndsWith("/product.aspx"))
                    navHome.Attributes["class"] = "active";
                else if (path.Contains("orderhistory"))
                    navOrders.Attributes["class"] = "active";
                else if (path.Contains("about"))
                    navAbout.Attributes["class"] = "active";
                else if (path.Contains("feedback"))
                    navFeedback.Attributes["class"] = "active";
            }
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["LANG"] = ddlLanguage.SelectedValue;
            Response.Redirect(Request.RawUrl);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }
    }
}

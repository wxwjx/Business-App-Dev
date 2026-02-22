using System;
using System.Web.UI;

namespace Business_App_Dev
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LANG"] == null) Session["LANG"] = "en";

            string lang = (Session["LANG"] as string ?? "en").ToLowerInvariant();

            if (!IsPostBack)
            {
                var item = ddlLanguage.Items.FindByValue(lang);
                if (item != null)
                {
                    ddlLanguage.ClearSelection();
                    item.Selected = true;
                }
            }

            HighlightActiveTab();
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["LANG"] = (ddlLanguage.SelectedValue ?? "en").ToLowerInvariant();
            Response.Redirect(Request.RawUrl, true);
        }

        private void HighlightActiveTab()
        {
            string path = (Request.Url.AbsolutePath ?? "").ToLowerInvariant();

            navHome.Attributes.Remove("class");
            navOrders.Attributes.Remove("class");
            navAbout.Attributes.Remove("class");
            navHelp.Attributes.Remove("class");
            navFeedback.Attributes.Remove("class");

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
}
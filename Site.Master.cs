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

            string lang = (Session["LANG"] as string ?? "en").ToLowerInvariant();

            if (!IsPostBack)
            {
                if (ddlLanguage != null && ddlLanguage.Items != null)
                {
                    var item = ddlLanguage.Items.FindByValue(lang);
                    if (item != null)
                    {
                        ddlLanguage.ClearSelection();
                        item.Selected = true;
                    }
                }
            }

            HighlightActiveTab();
        }

        protected void ddlLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlLanguage == null) return;
            Session["LANG"] = (ddlLanguage.SelectedValue ?? "en").ToLowerInvariant();
            Response.Redirect(Request.RawUrl, true);
        }

        private void HighlightActiveTab()
        {
            string path = (Request.Url.AbsolutePath ?? "").ToLowerInvariant();

            if (navHome != null) navHome.Attributes.Remove("class");
            if (navOrders != null) navOrders.Attributes.Remove("class");
            if (navAbout != null) navAbout.Attributes.Remove("class");
            if (navHelp != null) navHelp.Attributes.Remove("class");
            if (navFeedback != null) navFeedback.Attributes.Remove("class");

            if (path.EndsWith("/product") || path.EndsWith("/product.aspx"))
                navHome.Attributes["class"] = "active";
            else if (path.Contains("orderhistory"))
                navOrders.Attributes["class"] = "active";
            else if (path.Contains("about"))
                navAbout.Attributes["class"] = "active";
            else if (path.Contains("help"))
                navHelp.Attributes["class"] = "active";
            else if (path.Contains("feedback"))
                navFeedback.Attributes["class"] = "active";
        }
    }
}
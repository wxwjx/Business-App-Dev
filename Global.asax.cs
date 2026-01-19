using System;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace Business_App_Dev
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Session_Start(object sender, EventArgs e)
        {
            // default language for every new session
            if (Session["LANG"] == null)
                Session["LANG"] = "en";
        }
    }
}

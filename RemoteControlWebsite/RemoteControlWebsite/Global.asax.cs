using System;
using System.Net;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

namespace RemoteControlWebsite
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
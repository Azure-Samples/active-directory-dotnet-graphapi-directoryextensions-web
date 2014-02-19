using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using System.IdentityModel.Tokens;

namespace OrgChart
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        // to allow @ in MVC URLs
        //protected void Application_AcquireRequestState(Object sender, EventArgs e)
        //{
        //    var realUrl = Request.ServerVariables["HTTP_URL"];
        //    Context.RewritePath(realUrl);
        //}

        protected void RefreshValidationSettings()
        {
            string configPath = AppDomain.CurrentDomain.BaseDirectory + "\\" + "Web.config";
            string metadataAddress =
                          ConfigurationManager.AppSettings["ida:FederationMetadataLocation"];
            ValidatingIssuerNameRegistry.WriteToConfig(metadataAddress, configPath);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RefreshValidationSettings();
        }
    }
}
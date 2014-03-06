// <copyright file="Global.asax.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace OrgChart
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Tracing;
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging;
    using Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Formatters;
    using Microsoft.WindowsAzure.ActiveDirectory.GraphClient;

    /// <summary>
    /// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    /// visit http://go.microsoft.com/?LinkId=9394801
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// start the application
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // setup logging to console and file
            GraphHelperEventTextFormatter formatter = new GraphHelperEventTextFormatter();
            var consoleListener = ConsoleLog.CreateListener(formatter);
            consoleListener.EnableEvents(GraphHelperEventSource.Log, System.Diagnostics.Tracing.EventLevel.Informational);
            var fileListener = RollingFlatFileLog.CreateListener(StringConstants.LogFile, 1024, "yyyy.MM.dd", Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks.RollFileExistsBehavior.Increment, Microsoft.Practices.EnterpriseLibrary.SemanticLogging.Sinks.RollInterval.None, formatter);
            fileListener.EnableEvents(GraphHelperEventSource.Log, System.Diagnostics.Tracing.EventLevel.Informational);
        }
    }
}
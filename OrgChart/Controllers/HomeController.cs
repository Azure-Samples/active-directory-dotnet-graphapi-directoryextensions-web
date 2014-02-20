using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OrgChart.Models;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OrgChart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // use ADAL library to connect to AAD tenant using fake parameters
            string baseGraphUri = StringConstants.baseGraphUri + StringConstants.tenant;
            GraphQuery graphCall = new GraphQuery();
            graphCall.apiVersion = StringConstants.apiVersion;
            graphCall.baseGraphUri = baseGraphUri;
            // get token using OAuth Authorization Code
            AzureADAuthentication aadAuthentication = new AzureADAuthentication();
            AuthenticationResult authenticationResult = aadAuthentication.GetAuthenticationResult(StringConstants.tenant,
                                             StringConstants.clientId, StringConstants.clientSecret,
                                             StringConstants.resource, StringConstants.authenticationEndpoint);
            if (authenticationResult != null)
            {
                ViewBag.Message = "Authentication succeeded!";
                // initialize view data based on default or query string UPN
                NameValueCollection queryValues = Request.QueryString;
                string strUpn = queryValues["upn"];
                // initialize graph
                graphCall.aadAuthentication = aadAuthentication;
                graphCall.aadAuthentication.aadAuthenticationResult = authenticationResult;
                // configure appropriate model                
                OrgChart.Models.Org org = new OrgChart.Models.Org(graphCall);
                // retrieve user containing all extensions
                ViewBag.extensionRegistryUser = org.getUserJson(StringConstants.getExtensionRegistryUser());
                // process any form submission requests
                string strFormAction = Request["submitButton"];
                // setup JObject for setuser
                JObject graphUser = new JObject();
                graphUser["userPrincipalName"] = Request["updateUPN"];
                graphUser["displayName"] = Request["updateDisplayName"];
                graphUser["managerUserPrincipalName"] = Request["updateManagerUPN"];
                graphUser["jobTitle"] = Request["updateJobTitle"];
                // enumerate extension attributes from JSON object
                foreach (JProperty property in ViewBag.extensionRegistryUser.Properties())
                {
                    if (property.Name.StartsWith(StringConstants.extensionPropertyPrefix))
                    {
                        graphUser[property.Name] = Request[property.Name];
                    }
                }
                // setup variables for create user 
                string strCreateUPN = Request["createUPN"];
                string strCreateMailNickname = Request["createMailNickname"];
                string strCreateDisplayName = Request["createDisplayName"];
                string strCreateManagerUPN = Request["createManagerUPN"];
                string strCreateJobTitle = Request["createJobTitle"];
                switch (strFormAction)
                {
                    case "Update":
                        // set display name, manager, job title, trio, skype for given UPN
                        org.setUser(graphUser);
                        if (graphUser[StringConstants.getExtension("trio")] != null) strUpn = Request["updateManagerUPN"]; // if we just updated trio, show the manager
                        else if (graphUser[StringConstants.getExtension("skype")] != null) strUpn = Request["updateUPN"]; // if we just updated skype, show the user
                        break;
                    case "Create":
                        // create user with given display name, UPN, and manager
                        org.createUser(strCreateUPN, strCreateMailNickname, strCreateDisplayName, strCreateManagerUPN, strCreateJobTitle);
                        strUpn = strCreateUPN;
                        break;
                    case "Delete":
                        // delete user with given UPN
                        org.deleteUser(Request["updateUPN"]);
                        break;
                }
                if (strUpn == null)
                {
                    // no UPN provided, get the UPN of the first user instead
                    strUpn = org.getFirstUpn();
                }
                if (strUpn != null)
                {
                    string strTrio = queryValues["trio"];
                    bool bTrio = (strTrio != null && String.Equals(strTrio, "true", StringComparison.CurrentCultureIgnoreCase));
                    ViewBag.ancestorsAndMainPerson = org.getAncestorsAndMain(strUpn, bTrio);
                    ViewBag.directsOfDirects = org.getDirectsOfDirects(strUpn, bTrio);
                }
            }
            else
            {
                ViewBag.Message = "Authentication Failed!";
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Here is where you learn about the app.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Here is where you learn about the authors.";

            return View();
        }

    }
}
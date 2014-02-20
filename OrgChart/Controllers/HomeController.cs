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
                // retrieve user containing all extensions (and add manager UPN)
                ViewBag.extensionRegistryUser = org.getUserJson(StringConstants.getExtensionRegistryUser());
                ViewBag.extensionRegistryUser["managerUserPrincipalName"] = "asuthar@msonline-setup.com";
                // setup JObject for setuser by enumerating registry user
                JObject graphUser = new JObject();
                foreach (JProperty property in ViewBag.extensionRegistryUser.Properties())
                {
                    if (property.Name.StartsWith(StringConstants.extensionPropertyPrefix) || StringConstants.standardAttributes.Contains(property.Name))
                    {
                        graphUser[property.Name] = Request[property.Name];
                    }
                }
                // process any form submission requests
                string strFormAction = Request["submitButton"];
                switch (strFormAction)
                {
                    case "Update":
                        // set display name, manager, job title, trio, skype for given UPN
                        org.setUser(graphUser);
                        // show the user, unless trio is set, then show the manager
                        strUpn = Request["userPrincipalName"];
                        if ((string)graphUser[StringConstants.getExtension("trio")] != "") strUpn = Request["managerUserPrincipalName"];
                        break;
                    case "Create":
                        // create user with given display name, UPN, and manager
                        org.createUser(graphUser);
                        strUpn = (string)graphUser["userPrincipalName"];
                        break;
                    case "Delete":
                        // delete user with given UPN
                        org.deleteUser((string)graphUser["userPrincipalName"]);
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
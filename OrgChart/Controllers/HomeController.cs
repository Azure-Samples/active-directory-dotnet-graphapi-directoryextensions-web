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
        /// <summary>
        /// The Index method is the main method called when the front page of the website is launched. This method:
        /// 1. authenticates the application
        /// 2. performs requested actions in response to form submissions
        /// 3. calls the model to retrieve user data
        /// 4. calls the view to display user data
        /// </summary>
        /// <returns>ActionResult (generally a View).</returns>
        public ActionResult Index()
        {
            string strErrors = "";
            // check if we have changed authentication parameters
            string strFormAction = Request["submitButton"];
            if(strFormAction == "applicationUpdate")
            {
                Org.WhichCred = Request["WhichCred"];
                StringConstants.ClientId = Request["AppId"];
                StringConstants.ClientSecret = Request["AppSecret"];
                StringConstants.AppObjectId = Request["AppObjectId"];
                StringConstants.Tenant = Request["AppTenant"];
            }
            // use ADAL library to connect to AAD tenant using authentication parameters
            string baseGraphUri = StringConstants.BaseGraphUri + StringConstants.Tenant;
            GraphQuery graphCall = new GraphQuery();
            graphCall.ApiVersion = StringConstants.ApiVersion;
            graphCall.BaseGraphUri = baseGraphUri;
            // get token using OAuth Authorization Code
            AzureADAuthentication aadAuthentication = new AzureADAuthentication();
            AuthenticationResult authenticationResult = aadAuthentication.GetAuthenticationResult(StringConstants.Tenant,
                                             StringConstants.ClientId, StringConstants.ClientSecret,
                                             StringConstants.Resource, StringConstants.AuthenticationEndpoint, ref strErrors);
            if (authenticationResult != null)
            {
                ViewBag.Message = "Authentication succeeded!";
                // initialize view data based on default or query string UPN
                NameValueCollection queryValues = Request.QueryString;
                string strUpn = queryValues["upn"];
                // initialize graph
                graphCall.aadAuthentication = aadAuthentication;
                graphCall.aadAuthentication.AadAuthenticationResult = authenticationResult;
                // configure appropriate model                
                OrgChart.Models.Org org = new OrgChart.Models.Org(graphCall);
                // retrieve user containing all extensions (and add manager UPN)
                ViewBag.ExtensionRegistryUser = org.GetUser(Org.GetExtensionRegistryUser());
                ViewBag.ExtensionRegistryUser["managerUserPrincipalName"] = org.GetUsersManager(Org.GetExtensionRegistryUser());
                // setup JObject for setuser by enumerating registry user
                JObject graphUser = new JObject();
                foreach (JProperty property in ViewBag.ExtensionRegistryUser.Properties())
                {
                    if (property.Name.StartsWith(StringConstants.ExtensionPropertyPrefix) || Org.StandardAttributes.Contains(property.Name))
                    {
                        string value = Request[property.Name];
                        graphUser[property.Name] = (value == "") ? null : value;
                    }
                }
                // strFormAction set at top of Index() to process auth parameter actions, process the rest of the actions here
                switch (strFormAction)
                {
                    case "userUpdate":
                        // set display name, manager, job title, trio, skype for given UPN
                        org.SetUser(graphUser, ref strErrors);
                        // show the user, unless trio is set, then show the manager
                        strUpn = Request["userPrincipalName"];
                        if ((string)graphUser[StringConstants.GetExtension("trio")] != "") strUpn = Request["managerUserPrincipalName"];
                        break;
                    case "userCreate":
                        // create user with given display name, UPN, and manager
                        org.CreateUser(graphUser, ref strErrors);
                        strUpn = (string)graphUser["userPrincipalName"];
                        break;
                    case "userDelete":
                        // delete user with given UPN
                        org.DeleteUser((string)graphUser["userPrincipalName"], ref strErrors);
                        break;
                    case "extensionCreate":
                        {
                            // register the passed extension
                            string strExtension = Request["Extension"];
                            if (org.RegisterExtension(strExtension, ref strErrors))
                            {
                                // set this extension value to registered on the "registry" object
                                ViewBag.ExtensionRegistryUser[StringConstants.GetExtension(strExtension)] = "reserved";
                                org.SetUser(ViewBag.ExtensionRegistryUser, ref strErrors);
                            }
                        }
                        break;
                }
                // may have changed attributes, extension values, extension registration, or tenant credentials, re-retrieve extension registry user
                ViewBag.ExtensionRegistryUser = org.GetUser(Org.GetExtensionRegistryUser());
                ViewBag.ExtensionRegistryUser["managerUserPrincipalName"] = org.GetUsersManager(Org.GetExtensionRegistryUser());
                // no UPN provided, get the UPN of the first user instead
                if (strUpn == null)
                {
                    strUpn = org.GetFirstUpn();
                }
                // initialize the ViewBag if we have a UPN
                if (strUpn != null)
                {
                    string strTrio = queryValues["trio"];
                    bool bTrio = (strTrio != null && String.Equals(strTrio, "true", StringComparison.CurrentCultureIgnoreCase));
                    ViewBag.AncestorsAndMainPerson = org.GetAncestorsAndMain(strUpn, bTrio, ref strErrors);
                    ViewBag.DirectsOfDirects = org.GetDirectsOfDirects(strUpn, bTrio, ref strErrors);
                    ViewBag.strErrors = strErrors;
                }
            }
            else
            {
                ViewBag.Message = "Authentication Failed!";
            }
            return View();
        }
        /// <summary>
        /// Generates the About page.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Here is where you learn about the app.";

            return View();
        }
        /// <summary>
        /// Generates the Contact page.
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Here is where you learn about the authors.";

            return View();
        }

    }
}
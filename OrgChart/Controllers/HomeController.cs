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
                Org.whichCred = Request["WhichCred"];
                StringConstants.clientId = Request["AppId"];
                StringConstants.clientSecret = Request["AppSecret"];
                StringConstants.AppObjectId = Request["AppObjectId"];
                StringConstants.tenant = Request["AppTenant"];
            }
            // use ADAL library to connect to AAD tenant using authentication parameters
            string baseGraphUri = StringConstants.baseGraphUri + StringConstants.tenant;
            GraphQuery graphCall = new GraphQuery();
            graphCall.apiVersion = StringConstants.apiVersion;
            graphCall.baseGraphUri = baseGraphUri;
            // get token using OAuth Authorization Code
            AzureADAuthentication aadAuthentication = new AzureADAuthentication();
            AuthenticationResult authenticationResult = aadAuthentication.GetAuthenticationResult(StringConstants.tenant,
                                             StringConstants.clientId, StringConstants.clientSecret,
                                             StringConstants.resource, StringConstants.authenticationEndpoint, ref strErrors);
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
                ViewBag.extensionRegistryUser = org.getUserJson(Org.getExtensionRegistryUser());
                ViewBag.extensionRegistryUser["managerUserPrincipalName"] = org.getUsersManager(Org.getExtensionRegistryUser());
                // setup JObject for setuser by enumerating registry user
                JObject graphUser = new JObject();
                foreach (JProperty property in ViewBag.extensionRegistryUser.Properties())
                {
                    if (property.Name.StartsWith(StringConstants.extensionPropertyPrefix) || Org.standardAttributes.Contains(property.Name))
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
                        org.setUser(graphUser, ref strErrors);
                        // show the user, unless trio is set, then show the manager
                        strUpn = Request["userPrincipalName"];
                        if ((string)graphUser[StringConstants.getExtension("trio")] != "") strUpn = Request["managerUserPrincipalName"];
                        break;
                    case "userCreate":
                        // create user with given display name, UPN, and manager
                        org.createUser(graphUser, ref strErrors);
                        strUpn = (string)graphUser["userPrincipalName"];
                        break;
                    case "userDelete":
                        // delete user with given UPN
                        org.deleteUser((string)graphUser["userPrincipalName"], ref strErrors);
                        break;
                    case "extensionCreate":
                        {
                            // register the passed extension
                            string strExtension = Request["Extension"];
                            if (org.registerExtension(strExtension, ref strErrors))
                            {
                                // set this extension value to registered on the "registry" object
                                ViewBag.extensionRegistryUser[StringConstants.getExtension(strExtension)] = "reserved";
                                org.setUser(ViewBag.extensionRegistryUser, ref strErrors);
                            }
                        }
                        break;
                }
                // may have changed attributes, extension values, extension registration, or tenant credentials, re-retrieve extension registry user
                ViewBag.extensionRegistryUser = org.getUserJson(Org.getExtensionRegistryUser());
                ViewBag.extensionRegistryUser["managerUserPrincipalName"] = org.getUsersManager(Org.getExtensionRegistryUser());
                // no UPN provided, get the UPN of the first user instead
                if (strUpn == null)
                {
                    strUpn = org.getFirstUpn();
                }
                // initialize the ViewBag if we have a UPN
                if (strUpn != null)
                {
                    string strTrio = queryValues["trio"];
                    bool bTrio = (strTrio != null && String.Equals(strTrio, "true", StringComparison.CurrentCultureIgnoreCase));
                    ViewBag.ancestorsAndMainPerson = org.getAncestorsAndMain(strUpn, bTrio, ref strErrors);
                    ViewBag.directsOfDirects = org.getDirectsOfDirects(strUpn, bTrio, ref strErrors);
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
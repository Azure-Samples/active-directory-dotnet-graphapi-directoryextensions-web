// <copyright file="HomeController.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace OrgChart.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.WindowsAzure.ActiveDirectory.GraphClient;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using OrgChart.Models;

    /// <summary>
    /// main controller provides methods for each page of the site
    /// </summary>
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
            string strErrors = string.Empty;
            
            // check if we have changed authentication parameters
            string strFormAction = this.Request["submitButton"];
            if (strFormAction == "applicationUpdate")
            {
                Org.WhichCred(this.Request["WhichCred"]);
                StringConstants.ClientId = this.Request["AppId"];
                StringConstants.ClientSecret = this.Request["AppSecret"];
                StringConstants.AppObjectId = this.Request["AppObjectId"];
                StringConstants.Tenant = this.Request["AppTenant"];
            }
            
            // use ADAL library to connect to AAD tenant using authentication parameters
            string baseGraphUri = StringConstants.BaseGraphUri + StringConstants.Tenant;
            GraphQuery graphCall = new GraphQuery();
            graphCall.ApiVersion = StringConstants.ApiVersion;
            graphCall.BaseGraphUri = baseGraphUri;
            
            // get token using OAuth Authorization Code
            AzureADAuthentication aadAuthentication = new AzureADAuthentication();
            AuthenticationResult authenticationResult = aadAuthentication.GetAuthenticationResult(
                StringConstants.Tenant,
                StringConstants.ClientId, 
                StringConstants.ClientSecret,
                StringConstants.Resource, 
                StringConstants.AuthenticationEndpoint, 
                ref strErrors);
            if (authenticationResult != null)
            {
                ViewBag.Message = "Authentication succeeded!";
                
                // initialize view data based on default or query string UPN
                NameValueCollection queryValues = Request.QueryString;
                string strUpn = queryValues["upn"];
                
                // initialize graph
                graphCall.aadAuthentication = aadAuthentication;
                graphCall.aadAuthentication.AadAuthenticationResult = authenticationResult;
                
                // configure org and extensions model objects
                OrgChart.Models.Org org = new OrgChart.Models.Org(graphCall);
                OrgChart.Models.DirectoryExtensions extensions = new OrgChart.Models.DirectoryExtensions(graphCall);
                
                // retrieve template user containing all extensions and add manager UPN
                ViewBag.ExtensionRegistryUser = extensions.GetExtensionRegistryUser(ref strErrors);
                ViewBag.ExtensionRegistryUser["managerUserPrincipalName"] = org.GetUsersManager(DirectoryExtensions.GetExtensionRegistryUserUpn());
                
                // setup JObject for setuser by enumerating registry user
                JObject graphUser = new JObject();
                foreach (JProperty property in ViewBag.ExtensionRegistryUser.Properties())
                {
                    if (property.Name.StartsWith(DirectoryExtensions.ExtensionPropertyPrefix) || Org.StandardAttributes().Contains(property.Name))
                    {
                        string value = this.Request[property.Name];
                        graphUser[property.Name] = (value == string.Empty) ? null : value;
                    }
                }
                
                // strFormAction set at top of Index() to process auth parameter actions, process the rest of the actions here
                switch (strFormAction)
                {
                    case "userUpdate":
                        // set display name, manager, job title, trio, skype for given UPN
                        extensions.SetUser(graphUser, ref strErrors);
                        
                        // show the user, unless trio is set, then show the manager
                        strUpn = this.Request["userPrincipalName"];
                        if ((string)graphUser[DirectoryExtensions.GetExtensionName("trio")] != string.Empty)
                        {
                            strUpn = this.Request["managerUserPrincipalName"];
                        }

                        break;
                    case "userCreate":
                        // create user with given display name, UPN, and manager, show the new user
                        extensions.CreateUser(graphUser, ref strErrors);
                        strUpn = (string)graphUser["userPrincipalName"];
                        break;
                    case "userDelete":
                        // delete user with given UPN
                        org.DeleteUser((string)graphUser["userPrincipalName"], ref strErrors);
                        break;
                    case "extensionCreate":
                        {
                            // register the passed extension
                            string strExtension = this.Request["Extension"];
                            if (extensions.RegisterExtension(strExtension, ref strErrors))
                            {
                                // set this extension value to "registered" on the "registry" object
                                ViewBag.ExtensionRegistryUser[DirectoryExtensions.GetExtensionName(strExtension)] = "reserved";
                                JObject returnedUser = extensions.SetUser(ViewBag.ExtensionRegistryUser, ref strErrors);
                            }
                        }

                        break;
                }
                
                // may have changed attributes, extension values, extension registration, or tenant credentials, re-retrieve extension registry user
                ViewBag.ExtensionRegistryUser = extensions.GetExtensionRegistryUser(ref strErrors);
                ViewBag.ExtensionRegistryUser["managerUserPrincipalName"] = org.GetUsersManager(DirectoryExtensions.GetExtensionRegistryUserUpn());

                // no UPN provided, get the UPN of the first user instead
                if (strUpn == null)
                {
                    strUpn = org.GetFirstUpn();
                }
                
                // initialize the ViewBag if we have a UPN
                if (strUpn != null)
                {
                    string strTrio = queryValues["trio"];
                    bool bTrio = strTrio != null && string.Equals(strTrio, "true", StringComparison.CurrentCultureIgnoreCase);
                    ViewBag.AncestorsAndMainPerson = org.GetAncestorsAndMain(strUpn, bTrio, ref strErrors);
                    ViewBag.DirectsOfDirects = org.GetDirectsOfDirects(strUpn, bTrio, ref strErrors);
                    ViewBag.strErrors = strErrors;
                }
            }
            else
            {
                ViewBag.Message = "Authentication Failed!";
            }

            return this.View();
        }
        
        /// <summary>
        /// Generates the About page.
        /// </summary>
        /// <returns>About View</returns>
        public ActionResult About()
        {
            ViewBag.Message = "Here is where you learn about the app.";

            return this.View();
        }

        /// <summary>
        /// Generates the Contact page.
        /// </summary>
        /// <returns>Contact View</returns>
        public ActionResult Contact()
        {
            ViewBag.Message = "Here is where you learn about the authors.";

            return this.View();
        }
    }
}
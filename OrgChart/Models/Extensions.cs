// <copyright file="Extensions.cs" company="Microsoft">
//     Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace OrgChart.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Microsoft.WindowsAzure.ActiveDirectory.GraphClient;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The Org class is the main model class that interacts with the GraphHelper library to make calls to Graph.
    /// The Org class is called by the HomeController to assemble data to send to the Home Index view.
    /// "Trio" is used to group/sort managers and subordinates for appropriate display in Home Index org chart view.
    /// </summary>
    public class Extensions
    {
        /// <summary>
        /// internal graph client for making REST calls to AAD
        /// </summary>
        private GraphQuery graphCall;

        /// <summary>
        /// Initializes a new instance of the <see cref="Extensions"/> class.
        /// </summary>
        /// <param name="gq">initializes graph client</param>
        public Extensions(GraphQuery gq)
        {
            this.graphCall = gq;
        }

        /// <summary>
        /// Create user (including extension attributes).  Foreach loop adds extension properties to object before calling CreateUserJSON.
        /// </summary>
        /// <param name="user">User object containing values to be set on new object.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns>created user</returns>
        public JObject CreateUser(JObject user, ref string strErrors)
        {
            // setup AadUser with standard attributes, accountEnabled, and password profile
            AadUser aadUser = new AadUser();
            aadUser.userPrincipalName = (string)user["userPrincipalName"];
            aadUser.displayName = (string)user["displayName"];
            aadUser.mailNickname = (string)user["mailNickname"];
            aadUser.jobTitle = (string)user["jobTitle"];
            aadUser.passwordProfile = new passwordProfile();
            aadUser.accountEnabled = true;
            aadUser.passwordProfile.forceChangePasswordNextLogin = true;
            aadUser.passwordProfile.password = "P@55w0rd!";

            // convert to JObject
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSettings);
            JObject newUser = JObject.FromObject(aadUser, serializer);

            // add supported extension values
            foreach (JProperty property in user.Properties())
            {
                // exclude unsupported attributes added by my application logic
                if (property.Name == "isManager" || property.Name == "managerUserPrincipalName")
                {
                    // skip
                }
                else if (property.Name.StartsWith(StringConstants.ExtensionPropertyPrefix))
                {
                    newUser[property.Name] = property.Value;
                }
            }

            // create user
            newUser = this.graphCall.CreateUserJSON(newUser, ref strErrors);
            
            // set manager
            if (newUser != null)
            {
                newUser["managerUserPrincipalName"] = user["managerUserPrincipalName"];
                newUser = this.SetUser(newUser, ref strErrors);
            }
            
            return newUser;
        }

        /// <summary>
        /// get user JSON object (including extension attributes)
        /// </summary>
        /// <param name="strUpn">user UPN</param>
        /// <returns>user object</returns>
        public JObject GetUser(string strUpn)
        {
            string strErrors = string.Empty;
            return this.graphCall.GetUserJson(strUpn, ref strErrors);
        }

        /// <summary>
        /// register an extension attribute, this version only registers a string extension attribute on users
        /// </summary>
        /// <param name="strExtension">extension name</param>
        /// <param name="strErrors">error return value</param>
        /// <returns>success or failure</returns>
        public bool RegisterExtension(string strExtension, ref string strErrors)
        {
            // setup the extension definition
            ExtensionDefinition extension = new ExtensionDefinition();
            extension.name = strExtension;
            extension.dataType = "String";
            extension.targetObjects.Add("User");

            // Execute the POST to create new extension
            ExtensionDefinition returnedExtension = this.graphCall.createExtension(extension, ref strErrors);
            if (returnedExtension != null)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Set user extension attributes and manager. Foreach loop adds extension properties to object before calling ModifyUserJSON.
        /// </summary>
        /// <param name="user">User object with attributes set as intended on the target object.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns>set object</returns>
        public JObject SetUser(JObject user, ref string strErrors)
        {
            // set new (or same) display name and job title
            JObject graphUser = this.graphCall.GetUserJson((string)user["userPrincipalName"], ref strErrors);
            graphUser["displayName"] = user["displayName"];
            graphUser["jobTitle"] = user["jobTitle"];
            
            // enumerate extension attributes from JSON object
            foreach (JProperty property in user.Properties())
            {
                if (property.Name.StartsWith(StringConstants.ExtensionPropertyPrefix))
                {
                    graphUser[property.Name] = user[property.Name];
                }
            }            

            bool bPass = this.graphCall.ModifyUserJSON("PATCH", graphUser, ref strErrors);
            if (!bPass)
            {
                return null;
            }
            
            // set/clear manager
            string updateManagerURI = this.graphCall.BaseGraphUri + "/users/" + (string)user["userPrincipalName"] + "/$links/manager?" + this.graphCall.ApiVersion;
            urlLink managerlink = new urlLink();
            string method;
            if ((string)user["managerUserPrincipalName"] != "NO MANAGER")
            {
                AadUser manager = this.graphCall.getUser((string)user["managerUserPrincipalName"], ref strErrors);
                if (manager == null)
                {
                    return null;
                }

                managerlink.url = this.graphCall.BaseGraphUri + "/directoryObjects/" + manager.objectId;
                method = "PUT";
            }
            else
            {
                managerlink.url = null;
                method = "DELETE";
            }
            
            bPass = bPass && this.graphCall.updateLink(updateManagerURI, method, managerlink, ref strErrors);
            return graphUser;
        }
    }
}
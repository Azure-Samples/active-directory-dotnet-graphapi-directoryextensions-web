// <copyright file="DirectoryExtensions.cs" company="Microsoft">
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
    public class DirectoryExtensions
    {
        /// <summary>
        /// indicates prefix that all extension attribute names share
        /// </summary>
        public const string ExtensionPropertyPrefix = "extension_";

        /// <summary>
        /// internal graph client for making REST calls to AAD
        /// </summary>
        private GraphQuery graphCall;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryExtensions"/> class.
        /// </summary>
        /// <param name="gq">initializes graph client</param>
        public DirectoryExtensions(GraphQuery gq)
        {
            this.graphCall = gq;
        }

        /// <summary>
        /// return full extension name given a short name
        /// </summary>
        /// <param name="strExtensionName">short extension name</param>
        /// <returns>name string</returns>
        public static string GetExtensionName(string strExtensionName)
        {
            string extension = ExtensionPropertyPrefix;
            string strippedClientId = StringConstants.ClientId.Replace("-", string.Empty);
            extension += strippedClientId;
            extension += "_";
            extension += strExtensionName;
            return extension;
        }

        /// <summary>
        /// this application uses this user to set the value "registered" on each extension this app registers
        /// </summary>
        /// <returns>user UPN</returns>
        public static string GetExtensionRegistryUserUpn()
        {
            string strAdmin = "admin@";
            strAdmin += StringConstants.Tenant;
            return strAdmin;
        }

        /// <summary>
        /// register an extension attribute on the Application specified by StringConstants.AppObjectId
        /// this version only registers a string extension attribute on users
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
            return returnedExtension != null;
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
                // exclude unsupported attributes added by application logic
                if (property.Name == "isManager" || property.Name == "managerUserPrincipalName")
                {
                    // skip
                }
                else if (property.Name.StartsWith(DirectoryExtensions.ExtensionPropertyPrefix))
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
        /// get user JSON object (including extension attributes) for object storing extensions registered by this module
        /// </summary>
        /// <param name="strErrors">error return value</param>
        /// <returns>user object</returns>
        public JObject GetExtensionRegistryUser(ref string strErrors)
        {
            string strUpn = DirectoryExtensions.GetExtensionRegistryUserUpn();
            return this.GetUser(strUpn, ref strErrors);
        }

        /// <summary>
        /// get user JSON object (including extension attributes)
        /// </summary>
        /// <param name="strUpn">user UPN</param>
        /// <param name="strErrors">error return value</param>
        /// <returns>user object</returns>
        public JObject GetUser(string strUpn, ref string strErrors)
        {
            return this.graphCall.GetUserJson(strUpn, ref strErrors);
        }

        /// <summary>
        /// get user's direct reports JSON objects (including extension attributes)
        /// </summary>
        /// <param name="strUpn">user UPN</param>
        /// <param name="strErrors">error return value</param>
        /// <returns>JUsers user object list</returns>
        public JUsers GetUsersDirectReports(string strUpn, ref string strErrors)
        {
            return this.graphCall.getUsersDirectReportsJson(strUpn, ref strErrors);
        }

        /// <summary>
        /// get user's manager JSON object (including extension attributes)
        /// </summary>
        /// <param name="strUpn">user UPN</param>
        /// <param name="strErrors">error return value</param>
        /// <returns>user object</returns>
        public JObject GetUsersManager(string strUpn, ref string strErrors)
        {
            return this.graphCall.getUsersManagerJson(strUpn, ref strErrors);
        }

        /// <summary>
        /// Set user extension attributes and manager based on passed JObject.
        /// </summary>
        /// <param name="user">User object with attributes set as intended on the target object.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns>set object</returns>
        public JObject SetUser(JObject user, ref string strErrors)
        {
            string managerUserPrincipalName = (string)user["managerUserPrincipalName"];

            // remove unsupported properties added by application logic
            user.Remove("isManager");
            user.Remove("managerUserPrincipalName");

            // set object with passed attributes
            bool bPass = this.graphCall.ModifyUserJSON("PATCH", user, ref strErrors);
            if (!bPass)
            {
                return null;
            }

            // set user manager
            bPass = this.SetUserManager((string)user["userPrincipalName"], managerUserPrincipalName, ref strErrors);
            return user;
        }
        
        /// <summary>
        /// Set user manager. 
        /// </summary>
        /// <param name="userPrincipalName">User UPN.</param>
        /// <param name="managerUserPrincipalName">Manager UPN.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns>set object</returns>
        public bool SetUserManager(string userPrincipalName, string managerUserPrincipalName, ref string strErrors)
        {
            // construct URI, method, and managerLink
            string updateManagerURI = this.graphCall.BaseGraphUri + "/users/" + userPrincipalName + "/$links/manager?" + this.graphCall.ApiVersion;
            urlLink managerlink = new urlLink();
            string method;
            if (managerUserPrincipalName != "NO MANAGER")
            {
                AadUser manager = this.graphCall.getUser(managerUserPrincipalName, ref strErrors);
                if (manager == null)
                {
                    return false;
                }

                managerlink.url = this.graphCall.BaseGraphUri + "/directoryObjects/" + manager.objectId;
                method = "PUT";
            }
            else
            {
                managerlink.url = null;
                method = "DELETE";
            }

            return this.graphCall.updateLink(updateManagerURI, method, managerlink, ref strErrors);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OrgChart.Models
{
    /// <summary>
    /// The Org class is the main model class that interacts with the GraphHelper library to make calls to Graph.
    /// The Org class is called by the HomeController to assemble data to send to the Home Index view.
    /// "Trio" is used to group/sort managers and subordinates for appropriate display in Home Index orgchart view.
    /// </summary>
    public class Org
    {
        // this user has the word "registered" set on every extension registered by this app
        public static string GetExtensionRegistryUser()
        {
            string strAdmin = "admin@";
            strAdmin += StringConstants.Tenant;
            return strAdmin;
        }
        public static string[] StandardAttributes = { "displayName", "jobTitle", "userPrincipalName", "mailNickname", "managerUserPrincipalName" };
        public static string WhichCred = "dxtest orgchart";

        // initialize the object
        private GraphQuery graphCall;
        public Org(GraphQuery gq)
        {
            graphCall = gq;
        }
        /// <summary>
        /// Create user (including extension attributes).  Foreach loop adds extension properties to object before calling CreateUserJson.
        /// </summary>
        /// <param name="user">User object containing values to be set on new object.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns></returns>
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
            foreach(JProperty property in user.Properties())
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
            newUser = graphCall.CreateUserJson(newUser, ref strErrors);
            // set manager
            if (newUser != null)
            {
                newUser["managerUserPrincipalName"] = user["managerUserPrincipalName"];
                newUser = SetUser(newUser, ref strErrors);
            }
            return newUser;
        }
        //delete user
        public void DeleteUser(string strUpdateUPN, ref string strErrors)
        {
            // set new (or same) display name
            AadUser user = graphCall.getUser(strUpdateUPN, ref strErrors);
            bool bPass = graphCall.modifyUser("DELETE", user, ref strErrors);
        }
        /// <summary>
        /// Retrieves chain of command for entity represented by strUPN.
        /// If bTrio is not set, this returns list of single item lists terminating with single item list containing CEO.
        /// If bTrio is set, this returns list of trio leader lists terminating with single item list containing CEO.
        /// </summary>
        /// <param name="strUPN">Main person we are displaying in the org chart.</param>
        /// <param name="bTrio">Whether we are displaying in trio mode.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns></returns>       
        public List<List<JObject>> GetAncestorsAndMain(String strUPN, bool bTrio, ref string strErrors)
        {
            List<List<JObject>> returnedListOfLists = new List<List<JObject>>();
            // preserve original error
            string strOriginalError = strErrors;
            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');
            for (int idxTrio = 0; idxTrio < arrayUPN.Length; idxTrio++)
            {
                // retrieve graph node for this person (or for each trio member) from graph
                String strMainUPN = arrayUPN[idxTrio];
                JObject graphUser = graphCall.GetUserJson(strMainUPN, ref strErrors);

                // TODO: this logic is dependent on trios being properly filled in at each level of hierarchy

                // enumerate graph users from the main person (or from each trio member) to the root
                int idxAncestorOrMain = 0;
                while (graphUser != null)
                {
                    // create a new AncestorOrMain trio list if processing non-trio member or first member of trio
                    if (idxTrio == 0)
                    {
                        returnedListOfLists.Insert(0, new List<JObject>());
                    }
                    // get next graph user
                    JObject graphUserParent = graphCall.getUsersManagerJson((string)graphUser["userPrincipalName"], ref strErrors);
                    // tag user with manager attribute
                    graphUser["managerUserPrincipalName"] = (graphUserParent != null) ? graphUserParent["userPrincipalName"] : "NO MANAGER";
                    // insert user at end of the correct AncestorOrMain trio list
                    JToken tokenTrio = null;
                    if (bTrio && graphUser.TryGetValue(StringConstants.GetExtension("trio"), out tokenTrio))
                    {
                        // trio mode and there is a trio set on this object, add to list each time through
                        returnedListOfLists.ElementAt(returnedListOfLists.Count - idxAncestorOrMain - 1).Add(graphUser);
                    }
                    else if (idxTrio == 0)
                    {
                        // otherwise, this user not part of a trio, just add the first time through
                        returnedListOfLists.ElementAt(returnedListOfLists.Count - idxAncestorOrMain - 1).Add(graphUser);
                    }
                    // detect infinite loop: user is own manager, skip, notify and allow user to fix
                    if ((graphUserParent != null) && ((string)graphUserParent["userPrincipalName"] == (string)graphUser["userPrincipalName"]))
                    {
                        strErrors += ((string)graphUser["userPrincipalName"] + " has itself as manager. Please resolve.\n");
                        break;
                    }
                    // set next graph user
                    graphUser = graphUserParent;
                    // increment the ancestor level
                    idxAncestorOrMain++;
                }
                // we know that root doesn't have a manager, restore original error
                if (graphUser == null)
                {
                    strErrors = strOriginalError;
                }
            }
            return returnedListOfLists;
        }
        /// <summary>
        /// Retrieves subordinates for entity represented by strUPN.
        /// Each direct subordinate is head of a list, with subordinates of that direct as elements of that list.
        /// </summary>
        /// <param name="strUPN">Main person we are displaying in the org chart.</param>
        /// <param name="bTrio">Whether we are displaying in trio mode.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns></returns>
        public List<List<JObject>> GetDirectsOfDirects(string strUPN, bool bTrio, ref string strErrors)
        {
            List<List<JObject>> returnedListOfLists = new List<List<JObject>>();

            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');

            // TODO: if in trio mode and only one passed member, do filtered query for rest of trio
            // TODO: more efficient to do this filtered query for rest of trio in GetAncestorsAndMain

            // now retrieve direct reports for single person or trio
            for (int i = 0; i < arrayUPN.Length; i++)
            {
                String strMainUPN = arrayUPN[i];
                JUsers directs = graphCall.getUsersDirectReportsJson(strMainUPN, ref strErrors);
                if (directs != null && directs.users != null)
                {
                    foreach (JObject directReport in directs.users)
                    {
                        // add a new list at front of list of lists
                        returnedListOfLists.Insert(0, new List<JObject>());
                        // tag the direct report with manager attribute
                        directReport["managerUserPrincipalName"] = strMainUPN;
                        // insert the direct report at front of newly inserted list
                        returnedListOfLists.ElementAt(0).Insert(0, directReport);
                        // get direct reports of the direct report (and tag manager state to color code managers among directs)
                        JUsers directsOfDirect = graphCall.getUsersDirectReportsJson((string)directReport["userPrincipalName"], ref strErrors);
                        directReport["isManager"] = (directsOfDirect.users.Count > 0);
                        foreach (JObject directOfDirect in directsOfDirect.users)
                        {
                            // tag each direct of direct with manager attribute and assume they are not managers (for purposes of coloring)
                            directOfDirect["managerUserPrincipalName"] = (string)directReport["userPrincipalName"];
                            directOfDirect["isManager"] = false;
                            returnedListOfLists.ElementAt(0).Add(directOfDirect);
                        }
                    }
                }
            }
            // sort the list of lists by trio
            //http://stackoverflow.com/questions/3309188/c-net-how-to-sort-a-list-t-by-a-property-in-the-object
            returnedListOfLists.Sort(
                delegate(List<JObject> x, List<JObject> y)
                {
                    JToken tokenTrioX = null;
                    bool bxTrio = x.ElementAt(0).TryGetValue(StringConstants.GetExtension("trio"), out tokenTrioX);
                    JToken tokenTrioY = null;
                    bool byTrio = y.ElementAt(0).TryGetValue(StringConstants.GetExtension("trio"), out tokenTrioY);
                    // if neither has a trio, they are equal
                    if (!bxTrio && !byTrio) return 0;
                    // if only one has a trio, that one comes first
                    else if (bxTrio && !byTrio) return -1;
                    else if (!bxTrio && byTrio) return 1;
                    // if both have trios, perform the comparison to determine which one comes first
                    else if (bxTrio && byTrio) return ((string)tokenTrioX).CompareTo((string)tokenTrioY);
                    else return 0;
                }
            );
            return returnedListOfLists;
        }
        // pick a user to display first if none selected by user
        public string GetFirstUpn()
        {
            string userPrincipalName = null;
            string strErrors = "";
            AadUsers users = graphCall.GetUsers(ref strErrors);
            if (users.user != null)
            {
                userPrincipalName = users.user[0].userPrincipalName;
            }
            return userPrincipalName;
        }
        // get user JSON object (including extension attributes)
        public JObject GetUser(string strUpn)
        {
            string strErrors = "";
            return graphCall.GetUserJson(strUpn, ref strErrors);
        }
        // get users manager
        public string GetUsersManager(string strUpn)
        {
            string strErrors = "";
            AadUser manager = graphCall.GetUsersManager(strUpn, ref strErrors);
            if (manager != null)
            {
                return manager.userPrincipalName;
            }
            else
            {
                return "NO MANAGER";
            }
        }
        // register an extension attribute
        public bool RegisterExtension(string strExtension, ref string strErrors)
        {
            // setup the extension definition
            ExtensionDefinition extension = new ExtensionDefinition();
            extension.name = strExtension;
            extension.dataType = "String";
            extension.targetObjects.Add("User");

            // Execute the POST to create new extension
            ExtensionDefinition returnedExtension = graphCall.createExtension(extension, ref strErrors);
            if(returnedExtension != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Set user extension attributes and manager. Foreach loop adds extension properties to object before calling ModifyUserJson.
        /// </summary>
        /// <param name="user">User object with attributes set as intended on the target object.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns></returns>
        public JObject SetUser(JObject user, ref string strErrors)
        {
            // set new (or same) display name and job title
            JObject graphUser = graphCall.GetUserJson((string)user["userPrincipalName"], ref strErrors);
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
            bool bPass = graphCall.ModifyUserJson("PATCH", graphUser, ref strErrors);
            if (!bPass)
            {
                return null;
            }
            // set/clear manager
            string updateManagerURI = graphCall.BaseGraphUri + "/users/" + (string)user["userPrincipalName"] + "/$links/manager?" + graphCall.ApiVersion;
            urlLink managerlink = new urlLink();
            string method;
            if ((string)user["managerUserPrincipalName"] != "NO MANAGER")
            {
                AadUser manager = graphCall.getUser((string)user["managerUserPrincipalName"], ref strErrors);
                if (manager == null)
                {
                    return null;
                }
                managerlink.url = graphCall.BaseGraphUri + "/directoryObjects/" + manager.objectId;
                method = "PUT";
            }
            else
            {
                managerlink.url = null;
                method = "DELETE";
            }
            bPass = (bPass && graphCall.updateLink(updateManagerURI, method, managerlink, ref strErrors));
            return graphUser;
        }
    }
}
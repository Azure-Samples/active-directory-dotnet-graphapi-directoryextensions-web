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
        public static string getExtensionRegistryUser()
        {
            string strAdmin = "admin@";
            strAdmin += StringConstants.tenant;
            return strAdmin;
        }
        public static string[] standardAttributes = { "displayName", "jobTitle", "userPrincipalName", "mailNickname", "managerUserPrincipalName" };
        public static string whichCred = "dxtest orgchart";

        private GraphQuery graphCall;
        // initialize the object
        public Org(GraphQuery gq)
        {
            graphCall = gq;
        }
        // create user with requested extension values
        public JObject createUser(JObject user, ref string strErrors)
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
            aadUser.passwordProfile.password = "P0rsche911";

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
                else if (property.Name.StartsWith(StringConstants.extensionPropertyPrefix))
                {
                    newUser[property.Name] = property.Value;
                }
            }
            
            // create user
            newUser = graphCall.CreateUser(newUser, ref strErrors);

            // set manager
            newUser["managerUserPrincipalName"] = user["managerUserPrincipalName"];
            newUser = setUser(newUser, ref strErrors);
            return newUser;
        }
        //delete user
        public void deleteUser(string strUpdateUPN, ref string strErrors)
        {
            // set new (or same) display name
            AadUser user = graphCall.getUser(strUpdateUPN, ref strErrors);
            bool bPass = graphCall.modifyUser("DELETE", user, ref strErrors);
        }
        /// <summary>
        /// This method retrieves the ancestors for the main entity displayed in the org chart.
        /// The org chart may display a single person as the main entity, in which case this will be a single chain of command to the CEO.
        /// The org chart may display a trio of people as the main entity. In this case, this list will be a list of each members managers up to the CEO.
        /// </summary>
        /// <param name="strUPN">Main person we are displaying in the org chart.</param>
        /// <param name="bTrio">Whether we are displaying in trio mode.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns></returns>       
        public List<List<JObject>> getAncestorsAndMain(String strUPN, bool bTrio, ref string strErrors)
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
                JObject graphUser = graphCall.getUserJson(strMainUPN, ref strErrors);

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
                    if (bTrio && graphUser.TryGetValue(StringConstants.getExtension("trio"), out tokenTrio))
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
        /// This method retrieves the subordinates for the main entity displayed in the org chart.
        /// Each direct subordinate is at the head of a list, with the subordinates of that direct as elements of that list.
        /// </summary>
        /// <param name="strUPN">Main person we are displaying in the org chart.</param>
        /// <param name="bTrio">Whether we are displaying in trio mode.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns></returns>
        public List<List<JObject>> getDirectsOfDirects(string strUPN, bool bTrio, ref string strErrors)
        {
            List<List<JObject>> returnedListOfLists = new List<List<JObject>>();

            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');

            // TODO: if in trio mode and only one passed member, do filtered query for rest of trio
            // TODO: more efficient to do this filtered query for rest of trio in getAncestorsAndMain

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
                    bool bxTrio = x.ElementAt(0).TryGetValue(StringConstants.getExtension("trio"), out tokenTrioX);
                    JToken tokenTrioY = null;
                    bool byTrio = y.ElementAt(0).TryGetValue(StringConstants.getExtension("trio"), out tokenTrioY);
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
        public string getFirstUpn()
        {
            string userPrincipalName = null;
            string strErrors = "";
            AadUsers users = graphCall.getUsers(ref strErrors);
            if (users.user != null)
            {
                userPrincipalName = users.user[0].userPrincipalName;
            }
            return userPrincipalName;
        }
        // get user JSON object
        public JObject getUserJson(string strUpn)
        {
            string strErrors = "";
            return graphCall.getUserJson(strUpn, ref strErrors);
        }
        // get users manager
        public string getUsersManager(string strUpn)
        {
            string strErrors = "";
            AadUser manager = graphCall.getUsersManager(strUpn, ref strErrors);
            if (manager != null)
            {
                return manager.userPrincipalName;
            }
            else
            {
                return "NO MANAGER";
            }
        }
        // register an extension
        public bool registerExtension(string strExtension, ref string strErrors)
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
        // set user extension attributes and manager
        public JObject setUser(JObject user, ref string strErrors)
        {
            // set new (or same) display name and job title
            JObject graphUser = graphCall.getUserJson((string)user["userPrincipalName"], ref strErrors);
            graphUser["displayName"] = user["displayName"];
            graphUser["jobTitle"] = user["jobTitle"];
            // enumerate extension attributes from JSON object
            foreach (JProperty property in user.Properties())
            {
                if (property.Name.StartsWith(StringConstants.extensionPropertyPrefix))
                {
                    graphUser[property.Name] = user[property.Name];
                }
            }            
            bool bPass = graphCall.modifyUserJson("PATCH", graphUser, ref strErrors);
            if (!bPass)
            {
                return null;
            }
            // set/clear manager
            string updateManagerURI = graphCall.baseGraphUri + "/users/" + (string)user["userPrincipalName"] + "/$links/manager?" + graphCall.apiVersion;
            urlLink managerlink = new urlLink();
            string method;
            if ((string)user["managerUserPrincipalName"] != "NO MANAGER")
            {
                AadUser manager = graphCall.getUser((string)user["managerUserPrincipalName"], ref strErrors);
                if (manager == null)
                {
                    return null;
                }
                managerlink.url = graphCall.baseGraphUri + "/directoryObjects/" + manager.objectId;
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
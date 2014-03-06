// <copyright file="Org.cs" company="Microsoft">
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
    public class Org
    {
        /// <summary>
        /// list of attributes for extraction from JSON objects
        /// </summary>
        private static string[] standardAttributes = { "displayName", "jobTitle", "userPrincipalName", "mailNickname", "managerUserPrincipalName" };

        /// <summary>
        /// indicates which set of credentials should be active in the browser form
        /// </summary>
        private static string whichCred = "dxtest orgchart";

        /// <summary>
        /// Initialized graph client object
        /// </summary>
        private GraphQuery graphCall;

        /// <summary>
        /// DirectoryExtensions object for making calls that register extension attributes, or read/write objects with extension attributes
        /// </summary>
        private DirectoryExtensions extensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Org"/> class.
        /// </summary>
        /// <param name="gq">initialized graph client object</param>
        public Org(GraphQuery gq)
        {
            this.graphCall = gq;
            this.extensions = new DirectoryExtensions(gq);
        }

        /// <summary>
        /// get standardAttributes
        /// </summary>
        /// <returns>returns standardAttributes</returns>
        public static string[] StandardAttributes()
        {
            return standardAttributes;
        }

        /// <summary>
        /// get whichCred
        /// </summary>
        /// <returns>returns whichCred</returns>
        public static string WhichCred()
        {
            return whichCred;
        }

        /// <summary>
        /// set whichCred
        /// </summary>
        /// <param name="newCred">new value for whichCred</param>
        /// <returns>returns whichCred</returns>
        public static string WhichCred(string newCred)
        {
            whichCred = newCred;
            return whichCred;
        }

        /// <summary>
        /// delete a user
        /// </summary>
        /// <param name="strUpdateUPN">user to delete</param>
        /// <param name="strErrors">error return value</param>
        public bool DeleteUser(string strUpdateUPN, ref string strErrors)
        {
            return this.graphCall.DeleteUser(strUpdateUPN, ref strErrors);
        }

        /// <summary>
        /// Retrieves chain of command for entity represented by UPN.
        /// If bTrio is not set, this returns list of single item lists terminating with single item list containing CEO.
        /// If bTrio is set, this returns list of trio leader lists terminating with single item list containing CEO.
        /// </summary>
        /// <param name="strUPN">Main person we are displaying in the org chart.</param>
        /// <param name="bTrio">Whether we are displaying in trio mode.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns>list of list of users</returns>       
        public List<List<JObject>> GetAncestorsAndMain(string strUPN, bool bTrio, ref string strErrors)
        {
            List<List<JObject>> returnedListOfLists = new List<List<JObject>>();
            
            // preserve original error
            string strOriginalError = strErrors;
            
            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');
            for (int idxTrio = 0; idxTrio < arrayUPN.Length; idxTrio++)
            {
                // retrieve graph node for this person (or for each trio member) from graph
                string strMainUPN = arrayUPN[idxTrio];
                JObject graphUser = this.extensions.GetUser(strMainUPN, ref strErrors);

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
                    JObject graphUserParent = this.extensions.GetUsersManager((string)graphUser["userPrincipalName"], ref strErrors);
                    
                    // tag user with manager attribute
                    graphUser["managerUserPrincipalName"] = (graphUserParent != null) ? graphUserParent["userPrincipalName"] : "NO MANAGER";
                    
                    // insert user at end of the correct AncestorOrMain trio list
                    JToken tokenTrio = null;
                    if (bTrio && graphUser.TryGetValue(DirectoryExtensions.GetExtensionName("trio"), out tokenTrio))
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
                        strErrors += (string)graphUser["userPrincipalName"] + " has itself as manager. Please resolve.\n";
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
        /// Retrieves subordinates for entity represented by UPN.
        /// Each direct subordinate is head of a list, with subordinates of that direct as elements of that list.
        /// </summary>
        /// <param name="strUPN">Main person we are displaying in the org chart.</param>
        /// <param name="bTrio">Whether we are displaying in trio mode.</param>
        /// <param name="strErrors">Error return value.</param>
        /// <returns>list of list of users</returns>
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
                string strMainUPN = arrayUPN[i];
                JUsers directs = this.extensions.GetUsersDirectReports(strMainUPN, ref strErrors);
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
                        JUsers directsOfDirect = this.extensions.GetUsersDirectReports((string)directReport["userPrincipalName"], ref strErrors);
                        directReport["isManager"] = directsOfDirect.users.Count > 0;
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
            // http://stackoverflow.com/questions/3309188/c-net-how-to-sort-a-list-t-by-a-property-in-the-object
            returnedListOfLists.Sort(
                delegate(List<JObject> x, List<JObject> y)
                {
                    JToken tokenTrioX = null;
                    bool bxTrio = x.ElementAt(0).TryGetValue(DirectoryExtensions.GetExtensionName("trio"), out tokenTrioX);
                    JToken tokenTrioY = null;
                    bool byTrio = y.ElementAt(0).TryGetValue(DirectoryExtensions.GetExtensionName("trio"), out tokenTrioY);
                    
                    if (!bxTrio && !byTrio)
                    {
                        // if neither has a trio, they are equal
                        return 0;
                    }
                    else if (bxTrio && !byTrio)
                    {
                        // if only one has a trio, that one comes first
                        return -1;
                    }
                    else if (!bxTrio && byTrio)
                    {
                        // if only one has a trio, that one comes first
                        return 1;
                    }                    
                    else if (bxTrio && byTrio)
                    {
                        // if both have trios, perform the comparison to determine which one comes first
                        return ((string)tokenTrioX).CompareTo((string)tokenTrioY);
                    }
                    else
                    {
                        return 0;
                    }
                });
            return returnedListOfLists;
        }

        /// <summary>
        /// pick a user to display first if none selected by user
        /// </summary>
        /// <returns>user UPN</returns>
        public string GetFirstUpn()
        {
            string userPrincipalName = null;
            string strErrors = string.Empty;
            AadUsers users = this.graphCall.GetUsers(ref strErrors);
            if (users.user != null)
            {
                userPrincipalName = users.user[0].userPrincipalName;
            }

            return userPrincipalName;
        }

        /// <summary>
        /// get users manager
        /// </summary>
        /// <param name="strUpn">user UPN</param>
        /// <returns>manager UPN</returns>
        public string GetUsersManager(string strUpn)
        {
            string strErrors = string.Empty;
            AadUser manager = this.graphCall.GetUsersManager(strUpn, ref strErrors);
            if (manager != null)
            {
                return manager.userPrincipalName;
            }
            else
            {
                return "NO MANAGER";
            }
        }
    }
}
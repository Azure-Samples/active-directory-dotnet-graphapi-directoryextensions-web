using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace OrgChart.Models
{
    public class AadExtendedUser : AadUser
    {
        public AadExtendedUser(AadUser user, string managerUPN)
        {
            accountEnabled = user.accountEnabled;
            assignedLicenses = user.assignedLicenses;
            //assignedPlans = user.assignedPlans; // no set provided
            otherMails = user.otherMails;
            passwordPolicies = user.passwordPolicies;
            passwordProfile = user.passwordProfile;
            preferredLanguage = user.preferredLanguage;
            //provisionedPlans = user.provisionedPlans; // no set provided
            usageLocation = user.usageLocation;
            userPrincipalName = user.userPrincipalName;
            city = user.city;
            country = user.country;
            department = user.department;
            dirSyncEnabled = user.dirSyncEnabled;
            displayName = user.displayName;
            facsimileTelephoneNumber = user.facsimileTelephoneNumber;
            givenName = user.givenName;
            jobTitle = user.jobTitle;
            lastDirSyncTime = user.lastDirSyncTime;
            mail = user.mail;
            mailNickname = user.mailNickname;
            mobile = user.mobile;
            objectId = user.objectId;
            objectType = user.objectType;
            ODataType = user.ODataType;
            physicalDeliveryOfficeName = user.physicalDeliveryOfficeName;
            postalCode = user.postalCode;
            provisioningErrors = user.provisioningErrors;
            //proxyAddresses = user.proxyAddresses; // no set provided
            state = user.state;
            streetAddress = user.streetAddress;
            surname = user.surname;
            telephoneNumber = user.telephoneNumber;
            
            // these no longer exist and are only accessed by calling getUserJson
            //trio = user.trio;
            //skype = user.skype;

            isManager = false;
            managerUserPrincipalName = managerUPN;
        }
        public bool isManager { get; set; }
        public string managerUserPrincipalName { get; set; }

    }

    public class Org
    {
        private GraphQuery graphCall;
        public Org(GraphQuery gq)
        {
            graphCall = gq;
        }
        public JObject createUser(string strCreateUPN, string strCreateMailNickname, string strCreateDisplayName, string strCreateManagerUPN, string strCreateJobTitle)
        {
            string strTrioLed = null;
            string strSkypeContact = null;

            AadUser user = new AadUser();
            user.userPrincipalName = strCreateUPN;
            user.displayName = strCreateDisplayName;
            user.mailNickname = strCreateMailNickname;
            user.jobTitle = strCreateJobTitle;
            user.passwordProfile = new passwordProfile();
            user.passwordProfile.forceChangePasswordNextLogin = true;
            user.passwordProfile.password = "P0rsche911";
            AadUser newUser = graphCall.createUser(user);
            JObject extendedUser = null;
            if (newUser != null)
            {
                extendedUser = setUser(strCreateUPN, strCreateDisplayName, strCreateManagerUPN, strCreateJobTitle, strTrioLed, strSkypeContact);
            }
            return extendedUser;
        }
        public void deleteUser(string strUpdateUPN)
        {
            // set new (or same) display name
            AadUser user = graphCall.getUser(strUpdateUPN);
            bool bPass = graphCall.modifyUser("DELETE", user);
        }
        // list with main person (or main trio) as the last item in list
        public List<List<JObject>> getAncestorsAndMain(String strUPN, bool bTrio)
        {
            List<List<JObject>> returnedListOfLists = new List<List<JObject>>();
            
            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');
            for (int idxTrio = 0; idxTrio < arrayUPN.Length; idxTrio++)
            {
                // retrieve graph node for this person (or for each trio member) from graph
                String strMainUPN = arrayUPN[idxTrio];
                JObject graphUser = graphCall.getUserJson(strMainUPN);

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
                    JObject graphUserParent = graphCall.getUsersManagerJson((string)graphUser["userPrincipalName"]);
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
                    // set next graph user
                    graphUser = graphUserParent;
                    // increment the ancestor level
                    idxAncestorOrMain++;
                }
            }
            return returnedListOfLists;
        }
        // get subordinates list of lists with ICs as single person lists and leads as multiple person lists
        public List<List<JObject>> getDirectsOfDirects(string strUPN, bool bTrio)
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
                JObjects directs = graphCall.getUsersDirectReportsJson(strMainUPN);
                if (directs.users != null)
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
                        JObjects directsOfDirect = graphCall.getUsersDirectReportsJson((string)directReport["userPrincipalName"]);
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
                    bool byTrio = x.ElementAt(0).TryGetValue(StringConstants.getExtension("trio"), out tokenTrioY);
                    // if neither has a trio, they are equal
                    if (!bxTrio && !byTrio) return 0;
                    // if only one has a trio, that one comes first
                    else if (bxTrio && !byTrio) return -1;
                    else if (!bxTrio && byTrio) return 1;
                    // if both have trios, perform the comparison to determine which one comes first
                    else if (bxTrio && byTrio) return ((string)tokenTrioX).CompareTo((string)tokenTrioX);
                    else return 0;
                }
            );
            return returnedListOfLists;
        }
        public string getFirstUpn()
        {
            string userPrincipalName = null;
            AadUsers users = graphCall.getUsers();
            if (users.user != null)
            {
                userPrincipalName = users.user[0].userPrincipalName;
            }
            return userPrincipalName;
        }
        public JObject setUser(string strUpdateUPN, string strUpdateDisplayName, string strUpdateManagerUPN, string strUpdateJobTitle, string strUpdateTrioLed, string strSkypeContact)
        {
            // set new (or same) display name and job title
            JObject graphUser = graphCall.getUserJson(strUpdateUPN);
            graphUser["displayName"] = strUpdateDisplayName;
            graphUser["jobTitle"] = strUpdateJobTitle;
            graphUser[StringConstants.getExtension("trio")] = strUpdateTrioLed;
            graphUser[StringConstants.getExtension("skype")] = strSkypeContact;
            bool bPass = graphCall.modifyUserJson("PATCH", graphUser);
            // set new (or same) manager if a valid manager
            if (strUpdateManagerUPN != "NO MANAGER")
            {
                string updateManagerURI = graphCall.baseGraphUri + "/users/" + strUpdateUPN + "/$links/manager?" + graphCall.apiVersion;
                AadUser manager = graphCall.getUser(strUpdateManagerUPN);
                urlLink managerlink = new urlLink();
                managerlink.url = graphCall.baseGraphUri + "/directoryObjects/" + manager.objectId;
                bPass = (bPass && graphCall.updateLink(updateManagerURI, "PUT", managerlink));
            }
            return graphUser;
        }
        
        // TODO: figure out how to implement observer pattern - publish subscribe mechanism, for differential query
		
        // these can only be done with a cache
        //public List<AadUser> matchPartialAlias(string strPartial); // (fast full-text lookups)
		//public AadUser cacheAADGraph(string strUPN); // (makes 2 passes to load kids and grandkids, return requested AADUser)
		//public AadUser cacheLinkedIn(string strUPN); // (load connections, schools, employers)
		//public List<AadUser> findShortestPath(); // (connections)
        //public List<AadUser> findSharedHistory(); // (schools, employers)
    }
}
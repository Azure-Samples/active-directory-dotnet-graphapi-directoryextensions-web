using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Neo4jClient;

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
            trio = user.trio;
            skype = user.skype;

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
        public AadUser createUser(string strCreateUPN, string strCreateMailNickname, string strCreateDisplayName, string strCreateManagerUPN, string strCreateJobTitle)
        {
            string strTrioLed = "";
            string strSkypeContact = "";
            AadUser user = new AadUser();
            user.userPrincipalName = strCreateUPN;
            user.displayName = strCreateDisplayName;
            user.mailNickname = strCreateMailNickname;
            user.jobTitle = strCreateJobTitle;
            user.passwordProfile = new passwordProfile();
            user.passwordProfile.forceChangePasswordNextLogin = true;
            user.passwordProfile.password = "P0rsche911";
            AadUser newUser = graphCall.createUser(user);
            if (newUser != null)
            {
                newUser = setUser(strCreateUPN, strCreateDisplayName, strCreateManagerUPN, strCreateJobTitle, strTrioLed, strSkypeContact);
            }
            return newUser;
        }
        public void deleteUser(string strUpdateUPN)
        {
            // set new (or same) display name
            AadUser user = graphCall.getUser(strUpdateUPN);
            bool bPass = graphCall.modifyUser("DELETE", user);
        }
        // list with main person (or main trio) as the last item in list
        public List<List<AadExtendedUser>> getAncestorsAndMain(String strUPN, bool bTrio)
        {
            List<List<AadExtendedUser>> returnedListOfLists = new List<List<AadExtendedUser>>();
            
            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');
            for (int idxTrio = 0; idxTrio < arrayUPN.Length; idxTrio++)
            {
                // retrieve graph node for this person (or for each trio member) from graph
                String strMainUPN = arrayUPN[idxTrio];
                AadUser graphUser = graphCall.getUser(strMainUPN);

                // TODO: this logic is dependent on trios being properly filled in at each level of hierarchy

                // enumerate graph users from the main person (or from each trio member) to the root
                int idxAncestorOrMain = 0;
                while (graphUser != null)
                {
                    // create a new AncestorOrMain trio list if processing non-trio member or first member of trio
                    if (idxTrio == 0)
                    {
                        returnedListOfLists.Insert(0, new List<AadExtendedUser>());
                    }
                    // get next graph user
                    AadUser graphUserParent = graphCall.getUsersManager(graphUser.userPrincipalName);
                    // create new extended user with info from graph
                    AadExtendedUser extendedUser = new AadExtendedUser(graphUser, (graphUserParent != null) ? graphUserParent.userPrincipalName : "NO MANAGER");
                    // insert new extended user at end of the correct AncestorOrMain trio list
                    if (bTrio && extendedUser.trio != null && extendedUser.trio != "")
                    {
                        // trio mode and there is a trio, add to list every time
                        returnedListOfLists.ElementAt(returnedListOfLists.Count - idxAncestorOrMain - 1).Add(extendedUser);
                    }
                    else if (idxTrio == 0)
                    {
                        // otherwise, this user not part of a trio, just add once (the same pass that list is added)
                        returnedListOfLists.ElementAt(returnedListOfLists.Count - idxAncestorOrMain - 1).Add(extendedUser);
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
        public List<List<AadExtendedUser>> getDirectsOfDirects(string strUPN, bool bTrio)
        {
            List<List<AadExtendedUser>> returnedListOfLists = new List<List<AadExtendedUser>>();

            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');

            // TODO: if in trio mode and only one passed member, do filtered query for rest of trio
            // TODO: more efficient to do this filtered query for rest of trio in getAncestorsAndMain

            // now retrieve direct reports for single person or trio
            for (int i = 0; i < arrayUPN.Length; i++)
            {
                String strMainUPN = arrayUPN[i];
                AadUsers directs = graphCall.getUsersDirectReports(strMainUPN);
                if (directs != null)
                {
                    foreach (AadUser directReport in directs.user)
                    {
                        // add a new list at front of list of lists
                        returnedListOfLists.Insert(0, new List<AadExtendedUser>());
                        // insert the direct report at front of newly inserted list
                        AadExtendedUser extendedDirectReport = new AadExtendedUser(directReport, strMainUPN);
                        returnedListOfLists.ElementAt(0).Insert(0, extendedDirectReport);
                        // get direct reports of the direct report (and get manager state to color code managers among the directs)
                        AadUsers directsOfDirects = graphCall.getUsersDirectReports(directReport.userPrincipalName);
                        extendedDirectReport.isManager = (directsOfDirects.user.Count > 0 ? true : false);
                        foreach (AadUser directOfDirect in directsOfDirects.user)
                        {
                            // add each direct of direct to the end of the list
                            AadExtendedUser extendedDirectOfDirect = new AadExtendedUser(directOfDirect, directReport.userPrincipalName);
                            returnedListOfLists.ElementAt(0).Add(extendedDirectOfDirect);
                        }
                    }
                }
            }
            // sort the list of lists by trio
            //http://stackoverflow.com/questions/3309188/c-net-how-to-sort-a-list-t-by-a-property-in-the-object
            returnedListOfLists.Sort(delegate(List<AadExtendedUser> x, List<AadExtendedUser> y)
            {
                bool bxTrio = (x.ElementAt(0).trio != null && x.ElementAt(0).trio != "");
                bool byTrio = (y.ElementAt(0).trio != null && y.ElementAt(0).trio != "");
                // if neither has a trio, they are equal
                if (!bxTrio && !byTrio) return 0;
                // if only one has a trio, that one comes first
                else if (bxTrio && !byTrio) return -1;
                else if (!bxTrio && byTrio) return 1;
                // if both have trios, perform the comparison to determine which one comes first
                else if (bxTrio && byTrio) return x.ElementAt(0).trio.CompareTo(y.ElementAt(0).trio);
                else return 0;
            });
            return returnedListOfLists;
        }
        public string getFirstUpn()
        {
            string userPrincipalName = null;
            AadUsers users = graphCall.getUsers();
            if (users != null)
            {
                userPrincipalName = users.user[0].userPrincipalName;
            }
            return userPrincipalName;
        }
        public AadUser setUser(string strUpdateUPN, string strUpdateDisplayName, string strUpdateManagerUPN, string strUpdateJobTitle, string strUpdateTrioLed, string strSkypeContact)
        {
            // set new (or same) display name and job title
            AadUser graphUser = graphCall.getUser(strUpdateUPN);
            graphUser.displayName = strUpdateDisplayName;
            graphUser.jobTitle = strUpdateJobTitle;
            graphUser.trio = strUpdateTrioLed;
            graphUser.skype = strSkypeContact;
            bool bPass = graphCall.modifyUser("PATCH", graphUser);
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
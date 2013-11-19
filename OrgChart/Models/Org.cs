using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;

namespace OrgChart.Models
{

    public class AadExtendedUser : AadUser
    {
        public AadExtendedUser(AadUser user)
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
        }
        public string trioLed { get; set; }
        public string linkedInURL { get; set; }
        public bool isManager { get; set; }
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
                newUser = setUser(strCreateUPN, strCreateDisplayName, strCreateManagerUPN, strCreateJobTitle);
            }
            return newUser;
        }
        public void deleteUser(string strUpdateUPN)
        {
            // set new (or same) display name
            AadUser user = graphCall.getUser(strUpdateUPN);
            bool bPass = graphCall.modifyUser("DELETE", user);
        }
        // list with main person as the last item in list
        public List<AadUser> getAncestorsAndMainPerson(string strUPN)
        {
            List<AadUser> returnedList = new List<AadUser>();
            AadUser user = graphCall.getUser(strUPN);
            while (user != null)
            {
                returnedList.Insert(0, user);
                user = graphCall.getUsersManager(user.userPrincipalName);
            }
            return returnedList;
        }

        // list with ICs as single person lists and leads as multiple person lists
        public List<List<AadExtendedUser>> getDirectsOfDirects(string strUPN)
        {
            List<List<AadExtendedUser>> returnedListOfLists = new List<List<AadExtendedUser>>();
            AadUsers directs = graphCall.getUsersDirectReports(strUPN);
            if (directs != null)
            {
                foreach (AadUser directReport in directs.user)
                {
                    // insert the direct report
                    AadExtendedUser extendedDirectReport = new AadExtendedUser(directReport);
                    returnedListOfLists.Insert(0, new List<AadExtendedUser>());
                    returnedListOfLists.ElementAt(0).Insert(0, extendedDirectReport);
                    // get direct reports of the direct report
                    AadUsers directsOfDirects = graphCall.getUsersDirectReports(directReport.userPrincipalName);
                    extendedDirectReport.isManager = (directsOfDirects.user.Count > 0 ? true : false);
                    foreach (AadUser directOfDirect in directsOfDirects.user)
                    {
                        AadExtendedUser extendedDirectOfDirect = new AadExtendedUser(directOfDirect);
                        returnedListOfLists.ElementAt(0).Add(extendedDirectOfDirect);
                    }
                }
            }
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
        public AadUser setUser(string strUpdateUPN, string strUpdateDisplayName, string strUpdateManagerUPN, string strUpdateJobTitle)
        {
            // set new (or same) display name and job title
            AadUser user = graphCall.getUser(strUpdateUPN);
            user.displayName = strUpdateDisplayName;
            user.jobTitle = strUpdateJobTitle;
            bool bPass = graphCall.modifyUser("PATCH", user);
            // set new (or same) manager if a valid manager
            if (strUpdateManagerUPN != "NO MANAGER")
            {
                string updateManagerURI = graphCall.baseGraphUri + "/users/" + strUpdateUPN + "/$links/manager?" + graphCall.apiVersion;
                AadUser manager = graphCall.getUser(strUpdateManagerUPN);
                urlLink managerlink = new urlLink();
                managerlink.url = graphCall.baseGraphUri + "/directoryObjects/" + manager.objectId;
                bPass = (bPass && graphCall.updateLink(updateManagerURI, "PUT", managerlink));
            }
            return user;
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
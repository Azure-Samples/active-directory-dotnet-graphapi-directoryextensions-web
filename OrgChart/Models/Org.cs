using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Neo4jClient;

namespace OrgChart.Models
{
    // for the neo4j graph database calls
    public class User
    {
        public String userPrincipalName { get; set; }
        public String trioLed { get; set; }
        public String linkedInUrl { get; set; }
    }

    public class AadExtendedUser : AadUser
    {
        public AadExtendedUser(AadUser user, String trio, String liurl)
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
            trioLed = trio;
            linkedInUrl = liurl;
            isManager = false;
        }
        public string trioLed { get; set; }
        public string linkedInUrl { get; set; }
        public bool isManager { get; set; }
    }

    public class Org
    {
        private GraphQuery graphCall;
        private GraphClient neo4jClient;
        public Org(GraphQuery gq, GraphClient gc)
        {
            graphCall = gq;
            neo4jClient = gc;
        }
        public AadUser createUser(string strCreateUPN, string strCreateMailNickname, string strCreateDisplayName, string strCreateManagerUPN, string strCreateJobTitle)
        {
            string strTrioLed = "";
            string strLinkedInUrl = "";
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
                newUser = setUser(strCreateUPN, strCreateDisplayName, strCreateManagerUPN, strCreateJobTitle, strTrioLed, strLinkedInUrl);
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
        public List<List<AadExtendedUser>> getAncestorsAndMain(String strUPN, bool bTrio)
        {
            List<List<AadExtendedUser>> returnedListOfLists = new List<List<AadExtendedUser>>();
            
            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');
            for (int idxTrio = 0; idxTrio < arrayUPN.Length; idxTrio++)
            {
                String strMainUPN = arrayUPN[idxTrio];
                // retrieve neo4j nodes from this person to root
                //START target=node(0)
                //MATCH (source:User)
                //WHERE source.userPrincipalName="bshah@msonline-setup.com" OR source.userPrincipalName="scottgu@msonline-setup.com"
                //MATCH p = shortestPath(source-[*]-target)
                //return NODES(p);
                var neo4jResults = neo4jClient.Cypher
                    .Start(new { target = neo4jClient.RootNode })
                    .Match("(source:User)")
                    .Where((User source) => source.userPrincipalName == strMainUPN)
                    .Match("p = shortestPath(source-[*]-target)")
                    .Return<IEnumerable<User>>("nodes(p)")
                    .Results;

                // retrieve graph node for this person from graph
                AadUser graphUser = graphCall.getUser(strMainUPN);

                // enumerate neo4j and graph users from the main person to the root
                int idxAncestorOrMain = 0;
                foreach (IEnumerable<User> list in neo4jResults)
                {
                    foreach (User neo4jUser in list)
                    {
                        // bomb out if we are out of graph users
                        if (graphUser == null) break;
                        // create a new AncestorOrMain trio list if processing non-trio member or first member of trio
                        if (idxTrio == 0)
                        {
                            returnedListOfLists.Insert(0, new List<AadExtendedUser>());
                        }
                        // create new extended user with info from graph and neo4j
                        AadExtendedUser extendedUser = new AadExtendedUser(graphUser, neo4jUser.trioLed, neo4jUser.linkedInUrl);
                        // insert new extended user at end of the correct AncestorOrMain trio list
                        if (bTrio && neo4jUser.trioLed != "")
                        {
                            // if trio mode and there is a trio, add to list every time
                            returnedListOfLists.ElementAt(returnedListOfLists.Count - idxAncestorOrMain - 1).Add(extendedUser);
                        }
                        else if (idxTrio == 0)
                        {
                            // otherwise, this user not part of a trio, just add once (the same pass that list is added)
                            returnedListOfLists.ElementAt(returnedListOfLists.Count - idxAncestorOrMain - 1).Add(extendedUser);
                        }
                        // get next graph user
                        graphUser = graphCall.getUsersManager(graphUser.userPrincipalName);
                        // increment the ancestor level
                        idxAncestorOrMain++;
                    }
                }
            }
            return returnedListOfLists;
        }
        // list with ICs as single person lists and leads as multiple person lists
        public List<List<AadExtendedUser>> getDirectsOfDirects(string strUPN, bool bTrio)
        {
            List<List<AadExtendedUser>> returnedListOfLists = new List<List<AadExtendedUser>>();

            // split comma delimited UPN list into UPNs
            string[] arrayUPN = strUPN.Split(',');
            for (int i = 0; i < arrayUPN.Length; i++)
            {
                String strMainUPN = arrayUPN[i];
                // retrieve neo4j nodes from main person down two levels of management
                //MATCH (manager:User)-[:MANAGES*1..2]->(subordinate:User)
                //WHERE manager.userPrincipalName = "cliffdi@msonline-setup.com"
                //RETURN subordinate
                var neo4jResults = neo4jClient.Cypher
                    .Match("(manager:User)-[:MANAGES*1..2]->(subordinate:User)")
                    .Where((User manager) => manager.userPrincipalName == strMainUPN)
                    .Return<IEnumerable<User>>("collect(subordinate)")
                    .Results;
                // enumerate neo4j users into map
                Dictionary<string, User> neo4jUsers = new Dictionary<string, User>();
                foreach (IEnumerable<User> list in neo4jResults)
                {
                    foreach (User neo4jUser in list)
                    {
                        neo4jUsers.Add(neo4jUser.userPrincipalName, neo4jUser);
                    }
                }
                AadUsers directs = graphCall.getUsersDirectReports(strMainUPN);
                if (directs != null)
                {
                    foreach (AadUser directReport in directs.user)
                    {
                        // retrieve corresponding neo4j object
                        User neo4jUser = neo4jUsers[directReport.userPrincipalName];
                        // add a new list at front of list of lists
                        returnedListOfLists.Insert(0, new List<AadExtendedUser>());
                        // insert the direct report at front of newly inserted list
                        AadExtendedUser extendedDirectReport = new AadExtendedUser(directReport, neo4jUser.trioLed, neo4jUser.linkedInUrl);
                        returnedListOfLists.ElementAt(0).Insert(0, extendedDirectReport);
                        // get direct reports of the direct report
                        AadUsers directsOfDirects = graphCall.getUsersDirectReports(directReport.userPrincipalName);
                        extendedDirectReport.isManager = (directsOfDirects.user.Count > 0 ? true : false);
                        foreach (AadUser directOfDirect in directsOfDirects.user)
                        {
                            // retrieve corresponding neo4j object
                            neo4jUser = neo4jUsers[directOfDirect.userPrincipalName];
                            // add each direct of direct to the end of the list
                            AadExtendedUser extendedDirectOfDirect = new AadExtendedUser(directOfDirect, neo4jUser.trioLed, neo4jUser.linkedInUrl);
                            returnedListOfLists.ElementAt(0).Add(extendedDirectOfDirect);
                        }
                    }
                }
            }
            // sort the list of lists by trioled
            //http://stackoverflow.com/questions/3309188/c-net-how-to-sort-a-list-t-by-a-property-in-the-object
            returnedListOfLists.Sort(delegate(List<AadExtendedUser> x, List<AadExtendedUser> y)
            {
                bool bxTrio = (x.ElementAt(0).trioLed != null && x.ElementAt(0).trioLed != "");
                bool byTrio = (y.ElementAt(0).trioLed != null && y.ElementAt(0).trioLed != "");
                // if neither has a trio, they are equal
                if (!bxTrio && !byTrio) return 0;
                // if only one has a trio, that one comes first
                else if (bxTrio && !byTrio) return -1;
                else if (!bxTrio && byTrio) return 1;
                // if both have trios, perform the comparison to determine which one comes first
                else if (bxTrio && byTrio) return x.ElementAt(0).trioLed.CompareTo(y.ElementAt(0).trioLed);
                else return 0;
            });
            return returnedListOfLists;
        }
        public string getFirstUpn(bool bUpdateCache)
        {
            string userPrincipalName = null;
            AadUsers users = graphCall.getUsers();
            if (users != null)
            {
                userPrincipalName = users.user[0].userPrincipalName;
            }
            if (bUpdateCache) // called at app start and just got all the users...
            {
                // iterate over all users to load into neo4j
                foreach (AadUser user in users.user)
                {
                    // declare new user object
                    var newUser = new User { userPrincipalName = user.userPrincipalName, trioLed = "", linkedInUrl = "" };
                    // MERGE doesn't support map properties, need to explicitly specify properties
                    string strMerge = @"(user:User { userPrincipalName: {newUser}.userPrincipalName, 
                                                     trioLed: {newUser}.trioLed, 
                                                     linkedInUrl: {newUser}.linkedInUrl
                                                   })";
                    // neo4j call to store user
                    neo4jClient.Cypher
                            .Merge(strMerge)
                            .WithParam("newUser", newUser)
                            .ExecuteWithoutResults();
                }
                // iterate again to create :MANAGES links
                foreach (AadUser user in users.user)
                {
                    //set WHERE string for this user
                    String strWhere1 = "u.userPrincipalName = \"";
                    strWhere1 += user.userPrincipalName;
                    strWhere1 += "\"";
                    String strMatch2;
                    String strWhere2;
                    String strLinkCreation;

                    // graph call to get manager
                    AadUser manager = graphCall.getUsersManager(user.userPrincipalName);

                    // set strings for node that will point to this user
                    if (manager != null)
                    {
                        strMatch2 = "(m:User)";
                        strWhere2 = "m.userPrincipalName = \"";
                        strWhere2 += manager.userPrincipalName;
                        strWhere2 += "\"";
                        strLinkCreation = "m-[:MANAGES]->u";
                    }
                    else
                    {
                        strMatch2 = "(m)";
                        strWhere2 = "NOT (m:User)";
                        strLinkCreation = "m-[:CONTAINS]->u";
                    }
                    // neo4j call to set :MANAGES or :CONTAINS link
                    neo4jClient.Cypher
                        .Match("(u:User)", strMatch2)
                        .Where(strWhere1)
                        .AndWhere(strWhere2)
                        .CreateUnique(strLinkCreation)
                        .ExecuteWithoutResults();
                }
            }
            return userPrincipalName;
        }
        public AadUser setUser(string strUpdateUPN, string strUpdateDisplayName, string strUpdateManagerUPN, string strUpdateJobTitle, string strUpdateTrioLed, string strUpdateLinkedInUrl)
        {
            // set new (or same) display name and job title
            AadUser graphUser = graphCall.getUser(strUpdateUPN);
            graphUser.displayName = strUpdateDisplayName;
            graphUser.jobTitle = strUpdateJobTitle;
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
            // set extension data in neo4j
            neo4jClient.Cypher
                .Match("(user:User)")
                .Where((User user) => user.userPrincipalName == graphUser.userPrincipalName)
                .Set("user = {neo4jUser}")
                .WithParam("neo4jUser", new User { userPrincipalName = graphUser.userPrincipalName, trioLed = strUpdateTrioLed, linkedInUrl = strUpdateLinkedInUrl })
                .ExecuteWithoutResults();

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
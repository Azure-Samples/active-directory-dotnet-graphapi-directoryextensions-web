using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;

namespace OrgChart.Models
{
    public class Org
    {
        private GraphQuery graphCall;
        public Org(GraphQuery gq)
        {
            graphCall = gq;
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
        public List<List<AadUser>> getDirectsOfDirects(string strUPN)
        {
            List<List<AadUser>> returnedListOfLists = new List<List<AadUser>>();
            AadUsers directs = graphCall.getUsersDirectReports(strUPN);
            foreach (AadUser directReport in directs.user)
            {
                returnedListOfLists.Insert(0, new List<AadUser>());
                returnedListOfLists.ElementAt(0).Insert(0, directReport);
                AadUsers directsOfDirects = graphCall.getUsersDirectReports(directReport.userPrincipalName);
                foreach (AadUser directOfDirect in directsOfDirects.user)
                {
                    returnedListOfLists.ElementAt(0).Add(directOfDirect);
                }
            }
            return returnedListOfLists;
        }

        //public AadUser setUser();// (set attributes, extended attributes, parent)
		//public AadUser newUser(); // (set attributes, extended attributes, parent)
        
        // TODO: figure out how to implement observer pattern - publish subscribe mechanism, for differential query
		
        // these can only be done with a cache
        //public List<AadUser> matchPartialAlias(string strPartial); // (fast full-text lookups)
		//public AadUser cacheAADGraph(string strUPN); // (makes 2 passes to load kids and grandkids, return requested AADUser)
		//public AadUser cacheLinkedIn(string strUPN); // (load connections, schools, employers)
		//public List<AadUser> findShortestPath(); // (connections)
        //public List<AadUser> findSharedHistory(); // (schools, employers)
    }
}
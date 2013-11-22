using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ActiveDirectory.GraphHelper;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OrgChart.Models;
using System.Collections.Specialized;
using Neo4jClient;

namespace OrgChart.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // use ADAL library to connect to AAD tenant
            string baseGraphUri = StringConstants.baseGraphUri + StringConstants.tenant;
            GraphQuery graphCall = new GraphQuery();
            graphCall.apiVersion = StringConstants.apiVersion;
            graphCall.baseGraphUri = baseGraphUri;
            // get token using OAuth Authorization Code
            AzureADAuthentication aadAuthentication = new AzureADAuthentication();
            AuthenticationResult authenticationResult = aadAuthentication.GetAuthenticationResult(StringConstants.tenant,
                                             StringConstants.clientId, StringConstants.clientSecret,
                                             StringConstants.resource, StringConstants.authenticationEndpoint);
            if (authenticationResult != null)
            {
                ViewBag.Message = "Authentication succeeded!";
                // initialize graph
                graphCall.aadAuthentication = aadAuthentication;
                graphCall.aadAuthentication.aadAuthenticationResult = authenticationResult;
                // connect to neo4j
                GraphClient neo4jClient = new GraphClient(new Uri("http://OrgChart2:6IyKHclsfCHR6nOB9Eg5@OrgChart2.sb01.stations.graphenedb.com:24789/db/data/"));
                neo4jClient.Connect();
                // configure appropriate model                
                OrgChart.Models.Org org = new OrgChart.Models.Org(graphCall, neo4jClient);
                // process any form submission requests
                string strFormAction = Request["submitButton"];
                string strUpdateUPN = Request["updateUPN"];
                string strUpdateDisplayName = Request["updateDisplayName"];
                string strUpdateManagerUPN = Request["updateManagerUPN"];
                string strUpdateJobTitle = Request["updateJobTitle"];
                string strUpdateTrioLed = Request["updateTrioLed"];
                string strUpdateLinkedInUrl = Request["updateLinkedInUrl"];
                string strCreateUPN = Request["createUPN"];
                string strCreateMailNickname = Request["createMailNickname"];
                string strCreateDisplayName = Request["createDisplayName"];
                string strCreateManagerUPN = Request["createManagerUPN"];
                string strCreateJobTitle = Request["createJobTitle"];
                switch (strFormAction)
                {
                    case "Update":
                        // set display name and manager for given UPN
                        org.setUser(strUpdateUPN, strUpdateDisplayName, strUpdateManagerUPN, strUpdateJobTitle, strUpdateTrioLed, strUpdateLinkedInUrl);
                        break;
                    case "Create":
                        // create user with given display name, UPN, and manager
                        org.createUser(strCreateUPN, strCreateMailNickname, strCreateDisplayName, strCreateManagerUPN, strCreateJobTitle);
                        break;
                    case "Delete":
                        // delete user with given UPN
                        org.deleteUser(strUpdateUPN);
                        break;
                }

                // initialize view data based on default or query string UPN
                NameValueCollection queryValues = Request.QueryString;
                string strUpn = queryValues["upn"];
                if (strUpn == null)
                {
                    // no UPN provided, get the UPN of the first user instead (optionally building neo4j from graph)
                    strUpn = org.getFirstUpn(false);
                }
                ViewBag.ancestorsAndMainPerson = org.getAncestorsAndMainPerson(strUpn);
                ViewBag.directsOfDirects = org.getDirectsOfDirects(strUpn);
            }
            else
            {
                ViewBag.Message = "Authentication Failed!";
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Here is where you learn about the app.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Here is where you learn about the authors.";

            return View();
        }

    }
}
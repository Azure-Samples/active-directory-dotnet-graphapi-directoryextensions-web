using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Clients;
using Newtonsoft.Json;
using ExtensionMethods;
using Newtonsoft.Json.Linq;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphClient
{
    public class GraphQuery
    {
        // Graph API version
        public string ApiVersion;

        // The Graph service endpoint for a tenant
        public string BaseGraphUri;

        public AuthenticationResult AadAuthenticationResult;

        public AzureADAuthentication aadAuthentication;

        public AadUser getUser(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() || 
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
            //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);
            
            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
                       
            string uri = this.BaseGraphUri + "/users/" + userId + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadUser));
                        AadUser getUser = (AadUser)(ser.ReadObject(stream));
                        return getUser;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public JObject GetUserJson(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + userId + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        string payload;
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            payload = reader.ReadToEnd();
                        }
                        return JObject.Parse(payload);
                    }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUser GetUsersManager(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/users/" + userId + "/manager" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadUser));
                        AadUser getUser = (AadUser)(ser.ReadObject(stream));
                        return getUser;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public JObject getUsersManagerJson(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/users/" + userId + "/manager" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        string payload;
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            payload = reader.ReadToEnd();
                        }

                        return JObject.Parse(payload);
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUsers getUsersDirectReports(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + userId + "/directReports" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadUsers));
                        AadUsers userList = ser.ReadObject(stream) as AadUsers;
                        return userList;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public JUsers getUsersDirectReportsJson(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + userId + "/directReports" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        string payload;
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                        {
                            payload = reader.ReadToEnd();
                        }
                        return JObject.Parse(payload).ToObject<JUsers>();
                    }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadGroups getUsersGroupMembership(string userId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + userId + "/memberOf" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadGroups));
                        AadGroups groupList = ser.ReadObject(stream) as AadGroups;
                        return groupList;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUsers GetUsers(ref string strErrors)
        {

            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers userList = ser.ReadObject(stream) as AadUsers;
                            return userList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUsers GetUsers(AadFilter filter, int pageSize, ref string strErrors)
        {
            //maximum number of objects for a filtered search = 999
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users" + "?$top=" + pageSize.ToString()
                         + "&$filter=" + filter.objectProperty + " " + filter.operand + " '" + filter.propertyValue + "'"
                         + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers userList = ser.ReadObject(stream) as AadUsers;
                            return userList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUsers GetUsers(int pageSize, ref string strErrors)
        {

            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users" + "?$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers userList = ser.ReadObject(stream) as AadUsers;
                            return userList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        // this method supports getting the next link with the default pageSize = 99
        public AadUsers GetUsers(string nextLink, ref string strErrors)
        {

            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/" + nextLink + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers userList = ser.ReadObject(stream) as AadUsers;
                            return userList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        // this method supports getting the next link and allows pageSize to be specified
        public AadUsers GetUsers(string nextLink, int pageSize, ref string strErrors)
        {

            // check if token is expired or about to expire in 2 minutes
            
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/" + nextLink + "&$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers userList = ser.ReadObject(stream) as AadUsers;
                            return userList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUsers GetUsers(int pageSize, string nextLink, string orderbyProperty, ref string strErrors)
        {
            // validate pageSize arguments (must be 1-999)
            if (pageSize <= 0 || pageSize >= 1000)
                return null;
      
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = "";
            if (nextLink == "" || nextLink == null)
               uri = this.BaseGraphUri + "/users"
                         + "?$top=" + pageSize.ToString()
                         + "&" + "$orderby=" + orderbyProperty
                         + "&" + this.ApiVersion;
            else
                uri = this.BaseGraphUri + "/" + nextLink
                      + "&$top=" + pageSize.ToString()
                      + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers userList = ser.ReadObject(stream) as AadUsers;
                            return userList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public bool getUsersPhoto(string userId, string filePath, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))

                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + userId + "/thumbnailPhoto" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add(authnHeader);
                request.Method = "GET";
                request.ContentType = "image/Jpeg";
                using (var response = request.GetResponse())
                {
                    //Console.WriteLine("\nHeaders: " + response.Headers);
                    //Console.WriteLine("\nContent-Type: " + response.ContentType);
                    //Console.WriteLine("\nResponse Length: " + response.ContentLength);
                    //Console.WriteLine("\nType: " + response.GetType());
                    Stream stream = response.GetResponseStream();
                    Stream fileStream = File.Create(filePath);
                    stream.CopyTo(fileStream, (int)response.ContentLength);
                    fileStream.Close();
                    stream.Close();
                    return true;

                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }

        }

        public bool setUsersPhoto(string userId, string filePath, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))

                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + userId + "/thumbnailPhoto" + "?" + this.ApiVersion;


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add(authnHeader);
                request.Method = "PUT";
                request.ContentType = "image/Jpeg";

                Stream fileStream = File.OpenRead(filePath);
                if (fileStream != null)
                {
                    byte[] data = File.ReadAllBytes(filePath);
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            //Console.WriteLine("\nHeaders: " + response.Headers);
                            //Console.WriteLine("\nContent-Type: " + response.ContentType);
                            //Console.WriteLine("\nResponse Length: " + response.ContentLength);
                            //Console.WriteLine("\nType: " + response.GetType());
                            return true;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }

        }
        
        public AadGroup getGroup(string groupId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/groups/" + groupId + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadGroup));
                        AadGroup getGroup = (AadGroup)(ser.ReadObject(stream));
                        return getGroup;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadGroups getGroups(ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/groups" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {

                            var ser = new DataContractJsonSerializer(typeof(AadGroups));
                            AadGroups groupList = ser.ReadObject(stream) as AadGroups;
                            return groupList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadGroups getGroups(int pageSize, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/groups" + "?$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {

                            var ser = new DataContractJsonSerializer(typeof(AadGroups));
                            AadGroups groupList = ser.ReadObject(stream) as AadGroups;
                            return groupList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadGroups getGroups(int pageSize, string nextLink, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/" + nextLink + "&$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {

                            var ser = new DataContractJsonSerializer(typeof(AadGroups));
                            AadGroups groupList = ser.ReadObject(stream) as AadGroups;
                            return groupList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadContacts getContacts(int pageSize, ref string strErrors)
        {

            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/contacts" + "?$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadContacts));
                            AadContacts contactList = ser.ReadObject(stream) as AadContacts;
                            return contactList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadContacts getContacts(int pageSize, string nextLink, ref string strErrors)
        {

            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/" + nextLink + "&$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadContacts));
                            AadContacts contactList = ser.ReadObject(stream) as AadContacts;
                            return contactList;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadContact getContact(string contactId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/contacts/" + contactId + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadContact));
                        AadContact getContact = (AadContact)(ser.ReadObject(stream));
                        return getContact;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadObjects getGroupMembership(string groupId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/groups/" + groupId + "/members" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadObjects));
                        AadObjects getObjects = (AadObjects)(ser.ReadObject(stream));
                        return getObjects;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadObject getDirectoryObject(string objectType, string objectId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            if (objectType.ToLower() == "group" || objectType.ToLower() == "groups")
                objectType = "/groups";
            else if (objectType.ToLower() == "contact" || objectType.ToLower() == "contacts")
                objectType = "/contacts";
            else
                objectType = "/users";

            string uri = this.BaseGraphUri + objectType + "/" + objectId  + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadObject));
                        AadObject getObject = (AadObject)(ser.ReadObject(stream));
                        return getObject;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadObjects getDirectoryObjects(string objectType, int pageSize, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            if (objectType.ToLower() == "group" || objectType.ToLower() == "groups")
                objectType = "groups";
            else if (objectType.ToLower() == "contact" || objectType.ToLower() == "contacts")
                objectType = "contacts";
            else
                objectType = "users";

            if (pageSize >= 1000)
                pageSize = 999;

            string uri = this.BaseGraphUri + "/" + objectType + "?$top=" + pageSize.ToString() + "&" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadObjects));
                        AadObjects getObjects = (AadObjects)(ser.ReadObject(stream));
                        return getObjects;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadRoles getRoles(ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/roles" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {

                            var ser = new DataContractJsonSerializer(typeof(AadRoles));
                            AadRoles roles = ser.ReadObject(stream) as AadRoles;
                            return roles;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadRole getRole(string roleId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/roles/" + roleId + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {

                            var ser = new DataContractJsonSerializer(typeof(AadRole));
                            AadRole role = ser.ReadObject(stream) as AadRole;
                            return role;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUsers getRoleMembers(string roleId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/roles/" + roleId + "/members" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {

                            var ser = new DataContractJsonSerializer(typeof(AadUsers));
                            AadUsers users = ser.ReadObject(stream) as AadUsers;
                            return users;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadTenantDetails getTenantDetails(ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/tenantDetails" + "?" + ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadTenantDetails));
                            AadTenantDetails tenantInfo = ser.ReadObject(stream) as AadTenantDetails;
                            return tenantInfo;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadSubscribedSkus getSubscribedSkus(ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/subscribedSkus" + "?" + ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            var ser = new DataContractJsonSerializer(typeof(AadSubscribedSkus));
                            AadSubscribedSkus subscribedSkus = ser.ReadObject(stream) as AadSubscribedSkus;
                            return subscribedSkus;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public JObject AddExtensionPropertiesToObject(object aadObject, IDictionary<string, object> extensionValues)
        {
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSettings);
            JObject extendedObject = JObject.FromObject(aadObject, serializer);
            foreach (var kvpair in extensionValues)
            {
                string strValue = kvpair.Value as string;
                if (strValue != null)
                {
                    extendedObject[kvpair.Key] = strValue;
                }

                byte[] byteValue = kvpair.Value as byte[];
                if (byteValue != null)
                {
                    extendedObject[kvpair.Key] = Convert.ToBase64String(byteValue);
                }
            }

            return extendedObject;
        }

        public JObject CreateUserJSON(JObject user, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users" + "?" + this.ApiVersion;
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSettings);
            JObject user1 = JObject.FromObject(user, serializer);
            string body = JsonConvert.SerializeObject(user1, Formatting.None, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] data = encoding.GetBytes(body);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            string payload;
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                            {
                                payload = reader.ReadToEnd();
                            }

                            return JObject.Parse(payload);

                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public ExtensionDefinition createExtension(ExtensionDefinition extension, ref string strErrors)
        {
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/applications/" + StringConstants.AppObjectId + "/extensionProperties" + "?" + this.ApiVersion;

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
            JsonSerializer serializer = JsonSerializer.CreateDefault(jsonSettings);
            string body = JsonConvert.SerializeObject(JObject.FromObject(extension, serializer), Formatting.None, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] data = encoding.GetBytes(body);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            string payload;
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                            {
                                payload = reader.ReadToEnd();
                            }

                            return JObject.Parse(payload).ToObject<ExtensionDefinition>();
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUser CreateUser(AadUser user, ref string strErrors)
        {
            if (!this.ValidateAndRenewTokenIfRequired(ref strErrors))
            {
                return null;
            }

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users" + "?" + this.ApiVersion;

            //Setup AadUser object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = "";
            body = JsonConvert.SerializeObject(user, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] data = encoding.GetBytes(body);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                          DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadUser));
                          AadUser newUser = ser.ReadObject(stream) as AadUser;
                          return newUser;
                       }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public bool DeleteUser(string strDeleteUPN, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);
            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;
            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/users/" + strDeleteUPN + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                request.Headers.Add(authnHeader);
                request.Method = "DELETE";
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception(String.Format(
                            "Server error (HTTP {0}: {1}).",
                            response.StatusCode,
                            response.StatusDescription));
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public bool modifyUser(string method, AadUser user, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = "";

             if (method == "POST")
                uri = this.BaseGraphUri + "/users" + "?" + this.ApiVersion;
             else 
               uri = this.BaseGraphUri + "/users/" + user.userPrincipalName + "?" + this.ApiVersion;

            //Setup AadUser object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = "";
            if (uri.Contains("/users"))
                body = JsonConvert.SerializeObject(user, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method.ToUpper();

                if (method == "POST" || method == "PATCH" || method == "PUT")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if ((method == "GET" && response.StatusCode != HttpStatusCode.OK) ||
                        (method == "POST" && response.StatusCode != HttpStatusCode.Created) ||
                        (method == "PATCH" && response.StatusCode != HttpStatusCode.NoContent) ||
                        (method == "DELETE" && response.StatusCode != HttpStatusCode.NoContent))
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        return true;
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public bool ModifyUserJSON(string method, JObject user, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = "";

            if (method == "POST")
                uri = this.BaseGraphUri + "/users" + "?" + this.ApiVersion;
            else
                uri = this.BaseGraphUri + "/users/" + user["userPrincipalName"] + "?" + this.ApiVersion;

            //Setup user object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = "";
            if (uri.Contains("/users"))
                body = JsonConvert.SerializeObject(user, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method.ToUpper();

                if (method == "POST" || method == "PATCH" || method == "PUT")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if ((method == "GET" && response.StatusCode != HttpStatusCode.OK) ||
                        (method == "POST" && response.StatusCode != HttpStatusCode.Created) ||
                        (method == "PATCH" && response.StatusCode != HttpStatusCode.NoContent) ||
                        (method == "DELETE" && response.StatusCode != HttpStatusCode.NoContent))
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        return true;
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public bool modifyGroup(string method, AadGroup group, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/groups/" + group.objectId + "?" + this.ApiVersion;

            //Setup serialization
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            string body = "";
            if (uri.Contains("/groups"))
                body = JsonConvert.SerializeObject(group, jsonSettings);


            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method;

                if (method == "POST" || method == "PATCH" || method == "PUT")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if ((method == "GET" && response.StatusCode != HttpStatusCode.OK) ||
                        (method == "POST" && response.StatusCode != HttpStatusCode.Created) ||
                        (method == "PATCH" && response.StatusCode != HttpStatusCode.NoContent) ||
                        (method == "DELETE" && response.StatusCode != HttpStatusCode.NoContent))
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {                            
                            return true;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public AadGroup createGroup(AadGroup group, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/groups" + "?" + this.ApiVersion;
            string method = "POST";

            //Setup serialization
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            
            string body = "";
            if (uri.Contains("/groups"))
                body = JsonConvert.SerializeObject(group, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method;

                if (method == "POST" || method == "PATCH" || method == "PUT")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if ((method == "GET" && response.StatusCode != HttpStatusCode.OK) ||
                        (method == "POST" && response.StatusCode != HttpStatusCode.Created) ||
                        (method == "PATCH" && response.StatusCode != HttpStatusCode.NoContent) ||
                        (method == "DELETE" && response.StatusCode != HttpStatusCode.NoContent))
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadGroup));
                            AadGroup newGroup = ser.ReadObject(stream) as AadGroup;
                            return newGroup;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadUser licenseUser(AadUser user, userLicense license, ref string strErrors)
        {
            // return null, if the user's location location is not populated, then we can't assign a license            
            if (user.usageLocation == null)
                return null;
            
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            //Setup AadUser object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = JsonConvert.SerializeObject(license, jsonSettings);
            //Console.WriteLine(body);

            string method = "POST";

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + user.userPrincipalName + "/assignLicense" + "?" + StringConstants.ApiVersionPreview;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method;

                if (method == "POST" || method == "PATCH" || method == "PUT")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if ( (method == "POST" && response.StatusCode != HttpStatusCode.Created) && response.StatusCode != HttpStatusCode.OK )
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else if ((method == "PATCH") || (method == "PUT") || (method == "POST") )
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadUser));
                            AadUser newUser = ser.ReadObject(stream) as AadUser;
                            return newUser;
                        }
                    else 
                       return null;
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public bool licenseUser(AadUser user, string skuToAddOrUpdate, string skuToRemove, string[] disabledPlans, ref string strErrors)
        {
            // if incorrect parameters are passed in, return null
            if (user == null || (skuToAddOrUpdate == skuToRemove))
                return false;
            
            // create the addLicense object
            addLicense licenseObjectToAdd = new addLicense();

            // assign the subscriptionId to the addLicense object
            licenseObjectToAdd.skuId = skuToAddOrUpdate;

            //get the list of plans to remove and assign to the addLicense Object
            licenseObjectToAdd.disabledPlans = disabledPlans;

            //create a List of addLicense objects, and add the addLicense object
            List<addLicense> addLicensesList = new List<addLicense>();

            // if skuToAddOrUpdate is not empty, then add the license object, else remove the license Object
            if (skuToAddOrUpdate != "")
                addLicensesList.Add(licenseObjectToAdd);
            else
                addLicensesList.Clear();

            // assign addLicense and removeLicense objects to license object
            userLicense license = new userLicense();
            license.addLicenses = addLicensesList;

            // check to see if there's a skue to remove 
            if (skuToRemove != "")
            {
                string[] licenseToRemove = new string[1] { skuToRemove };
                license.removeLicenses = licenseToRemove;
            }
            else
                license.removeLicenses = new string[0] { };
            
            AadUser returnedUser = this.licenseUser(user, license, ref strErrors);

            if (returnedUser != null)
               return true;
            else
               return false;
        
        }

        public bool updateMembership(string RoleOrGroup, string action, string memberId, string parentId, ref string strErrors)
        { 
            string method;
            if (action.ToLower() == "add" || action.ToLower() == "post")
                method = "POST";
            else if (action.ToLower() == "delete" || action.ToLower() == "remove")
                method = "DELETE";
            else 
                return false;

            urlLink link = new urlLink();
            link.url = this.BaseGraphUri + "/directoryObjects/" + memberId;
            string graphUri = "";
            if (RoleOrGroup.ToUpper() == "GROUP")
              graphUri = this.BaseGraphUri + "/groups/" + parentId + "/$links" + "/members" + "?" + ApiVersion;
            else 
              graphUri = this.BaseGraphUri + "/roles/" + parentId + "/$links" + "/members" + "?" + ApiVersion;


            if (method == "DELETE")
            {
                link.url = "";
                if (RoleOrGroup.ToUpper() == "GROUP")
                  graphUri = this.BaseGraphUri + "/groups/" + parentId + "/$links" + "/members/" + memberId + "?" + ApiVersion;
                else
                  graphUri = this.BaseGraphUri + "/roles/" + parentId + "/$links" + "/members/" + memberId + "?" + ApiVersion;
            }

            if (this.updateLink(graphUri, method, link, ref strErrors))
                  return true;
            else
                  return false;
        }

        public bool updateLink(string uri, string method, urlLink link, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
       
            //Setup linked object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = JsonConvert.SerializeObject(link, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method;

                if (method == "POST" || method == "PATCH" || method == "PUT" || method == "DELETE")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    //if ((method == "POST") && ((response.StatusCode != HttpStatusCode.Created) ||
                    //                          (response.StatusCode != HttpStatusCode.OK) || (response.StatusCode != HttpStatusCode.NoContent)))
                    if (updateHasResponseError(response))
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                           return true;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public bool isMemberOf(string groupId, string memberId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/isMemberOf" + "?" + ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                request.Headers.Add(authnHeader);
                request.Method = "POST";
               
                string body = "{ " + "\"groupId\":" + "\"" + groupId + "\"" + ", " 
                                   + "\"memberId\":" + "\"" + memberId + "\"" + " }";

                byte[] data = encoding.GetBytes(body);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
    
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(isMemberOfResult));
                            isMemberOfResult result = ser.ReadObject(stream) as isMemberOfResult;
                            return result.value;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public string[] checkMemberGroups(string memberId, string[] groupIds, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
                        
            string uri = this.BaseGraphUri + "/users/" + memberId + "/checkMemberGroups" + "?" + ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";
                GroupIds groupIdList = new GroupIds();
                groupIdList.groupIds = groupIds;

                //Setup body
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.NullValueHandling = NullValueHandling.Ignore;
                string body = JsonConvert.SerializeObject(groupIdList, jsonSettings);

                byte[] data = encoding.GetBytes(body);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MemberGroupsData));
                            MemberGroupsData result = ser.ReadObject(stream) as MemberGroupsData;
                            return result.value;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public string[] getMemberGroups(string memberId, bool securityGroupsOnly, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/users/" + memberId + "/getMemberGroups" + "?" + ApiVersion;

            string body = "";
            if (securityGroupsOnly)
                body = "{\"securityEnabledOnly\":true}";
            else 
                body = "{\"securityEnabledOnly\":false}";

            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";

                byte[] data = encoding.GetBytes(body);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MemberGroupsData));
                            MemberGroupsData result = ser.ReadObject(stream) as MemberGroupsData;
                            return result.value;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public bool updateHasResponseError(HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.OK
               || response.StatusCode == HttpStatusCode.Accepted || response.StatusCode == HttpStatusCode.Created
               || response.StatusCode == HttpStatusCode.Found)
                return false;
            else
                return true;
        }

        public AadApplication createApplication(AadApplication application, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/applications" + "?" + this.ApiVersion;

            //Setup object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = "";
            body = JsonConvert.SerializeObject(application, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] data = encoding.GetBytes(body);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadApplication));
                            AadApplication newApplication = ser.ReadObject(stream) as AadApplication;
                            return newApplication;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public bool modifyApplication(string method, AadApplication application, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;
            string uri = this.BaseGraphUri + "/applications/" + application.objectId + "?" + this.ApiVersion;
            
            //Setup serialization
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;

            string body = "";
            if (uri.Contains("/applications"))
                body = JsonConvert.SerializeObject(application, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method;

                if (method == "POST" || method == "PATCH" || method == "PUT")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.Method = method;
                    request.ContentType = "application/json";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if ((method == "GET" && response.StatusCode != HttpStatusCode.OK) ||
                        (method == "POST" && response.StatusCode != HttpStatusCode.Created) ||
                        (method == "PATCH" && response.StatusCode != HttpStatusCode.NoContent) ||
                        (method == "DELETE" && response.StatusCode != HttpStatusCode.NoContent))
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            return true;
                        }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return false;
            }
        }

        public AadServicePrincipals getServicePrincipals(ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/servicePrincipals/" + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadServicePrincipals));
                        AadServicePrincipals getServicePrincipals = (AadServicePrincipals)(ser.ReadObject(stream));
                        return getServicePrincipals;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadServicePrincipal getServicePrincipal(string servicePrincipalObjectId, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                //if (this.authnResult.IsExpired() || this.authnResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/servicePrincipals/" + servicePrincipalObjectId + "?" + this.ApiVersion;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadServicePrincipal));
                        AadServicePrincipal getServicePrincipal = (AadServicePrincipal)(ser.ReadObject(stream));
                        return getServicePrincipal;
                    }
                }
            }
            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadServicePrincipal createServicePrincipal(AadServicePrincipal servicePrincipal, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/servicePrincipals" + "?" + this.ApiVersion;

            //Setup AadUser object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = "";
            body = JsonConvert.SerializeObject(servicePrincipal, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = "POST";
                request.ContentType = "application/json";
                byte[] data = encoding.GetBytes(body);
                request.ContentLength = data.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(AadServicePrincipal));
                            AadServicePrincipal newServicePrincipal = ser.ReadObject(stream) as AadServicePrincipal;
                            return newServicePrincipal;
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        public AadServicePrincipal modifyServicePrincipal(string method, AadServicePrincipal servicePrincipal, ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return null;

            string authnHeader = "Authorization: " + this.aadAuthentication.AadAuthenticationResult.AccessToken;

            string uri = this.BaseGraphUri + "/servicePrincipals/" + servicePrincipal.objectId + "?" + this.ApiVersion;

            //Setup object
            JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
            string body = "";
            body = JsonConvert.SerializeObject(servicePrincipal, jsonSettings);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                request.Headers.Add(authnHeader);
                request.Method = method.ToUpper();
                request.ContentType = "application/json";

                if (method == "PATCH")
                {
                    byte[] data = encoding.GetBytes(body);
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    }
                    else
                        using (var stream = response.GetResponseStream())
                        {
                            return this.getServicePrincipal(servicePrincipal.objectId, ref strErrors);
                        }
                }
            }

            catch (WebException webException)
            {
                GraphHelperEventSourceLogger.Log(webException, ref strErrors);
                return null;
            }
        }

        private bool ValidateAndRenewTokenIfRequired(ref string strErrors)
        {
            // check if token is expired or about to expire in 2 minutes
            if (this.aadAuthentication.AadAuthenticationResult.IsExpired() ||
                           this.aadAuthentication.AadAuthenticationResult.WillExpireIn(2))
                this.aadAuthentication.AadAuthenticationResult = this.aadAuthentication.GetNewAuthenticationResult(ref strErrors);

            if (this.aadAuthentication.AadAuthenticationResult == null)
                return false;

            return true;
        }
    }
 
    // filter object used for Graph Queries
    // e.g. https://graph.windows.net/contoso.com/user/$filter=State eq 'WA'
    // objectProperty = "State"  ,  operand = "eq",  propertyValue = "WA"
    // 
    public class AadFilter
    {
        public string objectProperty;
        public string operand;
        public string propertyValue;
    }
}
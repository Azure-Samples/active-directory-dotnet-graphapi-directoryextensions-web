using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;
using System.Diagnostics;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphClient
{
    public class AzureADAuthentication
    {
        public AuthenticationResult AadAuthenticationResult;

        // First Authn method OAuth Client credential flow (2-legged, S2S)
        //
        // 2-legged OAuth, describes a typical client-server scenario, without any user involvement. 
        // An example for such a scenario could be your Azure AD application accessing a consented tenant with no user interaction.
        //
        // On a conceptual level 2-legged OAuth simply consists of the first and last steps of 3-legged OAuth (see below):
        //      Client has signed up to the server and got his client credentials (also known as “consumer key and secret”)
        //      Client uses his client credentials (and empty token credentials) to access the protected resources on the server
        //
        public string GetToken(string tenant, string clientId, string clientSecret, string resource, string authnEndpoint, ref string strErrors)
        {
            string authString = authnEndpoint + tenant;
            AuthenticationContext authenticationContext = new AuthenticationContext(authString);
            try
            {
                ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
                AuthenticationResult authenticationResult = authenticationContext.AcquireToken(resource, clientCred);
                Console.WriteLine("Token: \nToken Type: {0} \nExpires: {1}", authenticationResult.AccessTokenType, authenticationResult.ExpiresOn);
                return authenticationResult.AccessToken;
            }
            catch (AdalException ex)
            {
                GraphHelperEventSourceLogger.Log(ex, ref strErrors);
                return "";
            }
        }

        // Second Authn method using OAuth Authorization Code grant flow (3-legged OAuth, user impersonation/delegation)
        //
        // 3-legged OAuth describes the scenario for which OAuth was originally developed: a resource owner wants to give a client 
        // access to a server without sharing his credentials (i.e. username/password).
        //
        // A typical example is a Facebook user (resource owner) who wants to give a facebook app (client) ability to write to her wall (server).
        //
        // On a conceptual level it works in the following way:
        //      Client has signed up to the server and got his client credentials (also known as “consumer key and secret”) ahead of time
        //      User wants to give the client access to his protected resources on the server
        //      Client retrieves the temporary credentials (also known as “request token”) from the server
        //      Client redirects the resource owner to the server
        //      Resource owner grants the client access to his protected resources on the server
        //      Server redirects the user back to the client
        //      Client uses the temporary credentials to retrieve the token credentials (also known as “access token”) from the server
        //      Client uses the token credentials to access the protected resources on the server
        //
        public string GetToken(string tenant, string clientId, Uri redirectUri, string resourceAppIdUri, string authnEndpoint, ref string strErrors)
             {
                // This method requests OAuth token using client code authn flow (3-legged)
                // user must authenticate
                string authString = authnEndpoint + tenant;
                AuthenticationContext authenticationContext = new AuthenticationContext(authString);
                AuthenticationResult userAuthnResult = null;
                try
                {                  
                   userAuthnResult = authenticationContext.AcquireToken(resourceAppIdUri, clientId, redirectUri);
                   Console.WriteLine("Token: \nToken Type: {0} \nExpires: {1}", userAuthnResult.AccessTokenType, userAuthnResult.ExpiresOn);
                   //Console.WriteLine("RefreshToken: {0}", userAuthnResult.RefreshToken);
                   return userAuthnResult.AccessToken;
                }
                catch (AdalException ex)
                {
                    GraphHelperEventSourceLogger.Log(ex, ref strErrors);
                    return "";
                }
             }

        // Third Authn method OAuth Client credential flow (2-legged, S2S)
        // returns entire Authentication Result
        public AuthenticationResult GetAuthenticationResult(string tenant, string clientId, string clientSecret,
                                                            string resource, string authnEndpoint, ref string strErrors)
        {   
            string authString = authnEndpoint + tenant;
            AuthenticationContext authenticationContext = new AuthenticationContext(authString);
            try
            {
                ClientCredential clientCred = new ClientCredential(clientId, clientSecret);
                AuthenticationResult authenticationResult = authenticationContext.AcquireToken(resource, clientCred);
                //Console.WriteLine("Token: \nToken Type: {0} \nExpires: {1}", authenticationResult.AccessTokenType, authenticationResult.ExpiresOn);
                return authenticationResult;
            }
            catch (AdalException ex)
            {
                GraphHelperEventSourceLogger.Log(ex, ref strErrors);
                return null;
            }
        }

        // Fourth Authn method OAuth Client Authorization Code grant flow (3-legged, user impersonation/delagion)
        // returns entire Authentication Result
        public AuthenticationResult GetAuthenticationResult(string tenant, string clientId, Uri redirectUri,
                                                              string resourceAppIdUri, string authnEndpoint, ref string strErrors)
        {
            // This method requests OAuth token using client code authn flow (3-legged)
            // user must authenticate

            string authString = authnEndpoint + tenant;
            AuthenticationContext authenticationContext = new AuthenticationContext(authString);
            AuthenticationResult userAuthnResult = null;

            try
            {
                userAuthnResult = authenticationContext.AcquireToken(resourceAppIdUri, clientId, redirectUri);
                Console.WriteLine("Token: \nToken Type: {0} \nExpires: {1}", userAuthnResult.AccessTokenType, userAuthnResult.ExpiresOn);
                //Console.WriteLine("RefreshToken: {0}", userAuthnResult.RefreshToken);
                return userAuthnResult;
            }
            catch (AdalException ex)
            {
                GraphHelperEventSourceLogger.Log(ex, ref strErrors);
                return null;
            }
        }

        // methods will get a new token.
        public AuthenticationResult GetNewAuthenticationResult(ref string strErrors)
        {
            // check which type of token to acquire by checking to see if a refresh token is available 
            // (indicating OAuth Authz code grant flow)
            if (this.AadAuthenticationResult.RefreshToken == null)
            {
                AzureADAuthentication appToken = new AzureADAuthentication();
                AuthenticationResult applicationAuthnResult = appToken.GetAuthenticationResult(StringConstants.Tenant,
                                            StringConstants.ClientId, StringConstants.ClientSecret,
                                            StringConstants.Resource, StringConstants.AuthenticationEndpoint, ref strErrors);
                return applicationAuthnResult;
            }
            else
            {
                AzureADAuthentication appToken = new AzureADAuthentication();
                AuthenticationResult userAuthnResult = appToken.GetAuthenticationResult(StringConstants.Tenant, 
                                            StringConstants.ClientId, StringConstants.RedirectUri,
                                            StringConstants.Resource, StringConstants.AuthenticationEndpoint, ref strErrors);
                return userAuthnResult;
            }
        
        }
    }
}

// Extension methods for Azure AD Authentication Library (ADAL)
// check if token is expired or about to expire in specified minutes)
namespace ExtensionMethods
{
    public static class MicrosoftIdentityModelExtensions
    {
        public static bool IsExpired(this AuthenticationResult authenticationResult)
        {
            return WillExpireIn(authenticationResult, 0);
        }

        public static bool WillExpireIn(this AuthenticationResult authenticationResult, double minutes)
        {
            DateTimeOffset datetimeoffset = DateTimeOffset.Now.UtcDateTime;
            return datetimeoffset.AddMinutes(minutes) > authenticationResult.ExpiresOn;
        }
    }
}

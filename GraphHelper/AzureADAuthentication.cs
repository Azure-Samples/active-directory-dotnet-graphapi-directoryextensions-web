using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading;
using System.Diagnostics;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphHelper
{
    public class AzureADAuthentication
    {
        public AuthenticationResult aadAuthenticationResult;

        // First Authn method OAuth Client credential flow (2-legged, S2S)
        // *
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
            catch (ActiveDirectoryAuthenticationException ex)
            {
                GraphHelperEventSourceLogger.Log(ex, ref strErrors);
                return "";
            }
        }

        // Second Authn method using OAuth Authorization Code grant flow (3-legged OAuth, user impersonation/delegation)
        // *
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
                catch (ActiveDirectoryAuthenticationException ex)
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
            catch (ActiveDirectoryAuthenticationException ex)
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
            catch (ActiveDirectoryAuthenticationException ex)
            {
                GraphHelperEventSourceLogger.Log(ex, ref strErrors);
                return null;
            }
        }

        // methods will get a new token.
        public AuthenticationResult getNewAuthenticationResult(ref string strErrors)
        {
            // check which type of token to acquire by checking to see if a refresh token is available 
            // (indicating OAuth Authz code grant flow)
            if (this.aadAuthenticationResult.RefreshToken == null)
            {
                AzureADAuthentication appToken = new AzureADAuthentication();
                AuthenticationResult applicationAuthnResult = appToken.GetAuthenticationResult(StringConstants.tenant,
                                            StringConstants.clientId, StringConstants.clientSecret,
                                            StringConstants.resource, StringConstants.authenticationEndpoint, ref strErrors);
                return applicationAuthnResult;
            }
            else
            {
                AzureADAuthentication appToken = new AzureADAuthentication();
                AuthenticationResult userAuthnResult = appToken.GetAuthenticationResult(StringConstants.tenant, 
                                            StringConstants.clientId, StringConstants.redirectUri,
                                            StringConstants.resource, StringConstants.authenticationEndpoint, ref strErrors);
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
        public static bool isExpired(this AuthenticationResult authenticationResult)
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

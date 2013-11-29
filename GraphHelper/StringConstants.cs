using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphHelper
{
    public class StringConstants
    {

        //application principal id
        //public const string GraphPrincipalId = "https://graph.windows.net";

        // resource that is used to request a token for
        public const string resource = "https://graph.windows.net";
        //public const string resource = "https://graph.27.MSODS.msol-nova.com";

        //this is the STS endpoint for Windows Azure AD
        public const string authenticationEndpoint = "https://login.windows.net/";
        //public const string authenticationEndpoint = "https://27.login.ORGID.msol-nova.com/";
        
        // The URL of Graph service endpoint 
        public const string baseGraphUri = "https://graph.windows.net/";
        //public const string baseGraphUri = "https://graph.27.MSODS.msol-nova.com/";

        // Graph API version
        public const string apiVersion = "api-version=2013-04-05";
        //public const string apiVersion = "api-version=1.21-preview";
        
        // Graph API preview version
        public const string apiVersionPreview = "api-version=2013-04-10-preview";

        // Next section is tenant and application specific configuraiton.
        
        // Your tenant objectId or tenant domain name – this can be any of a tenant's verified domain 
        public const string tenant = "msonlinesetup.onmicrosoft.com";
        //public const string tenant = "msonlinesetup.27.msods.msol-nova.com";

        // Config for OAuth client credentials (2-legged OAuth, S2S Authn flow)
        public const string clientId = "33b6cec1-7aa1-4574-9525-1f73a7c3b6e6";
        public const string clientSecret = "Kl/CdYAC32EmFxnC7Ek+hJHepi1hx9D9kHqxG5b0xVM=";

        // FIXTHIS: Config for OAuth Authorization Code grant flow (3-legged OAuth Auth flow - includes user authentication)
        public const string clientIdForUserAuthn = "7318ca25-0f32-4d77-8843-2c4bc2a44326";  //for user_impersonation
        public const string redirectUri = "https://localhost";

        //// Your tenant objectId or tenant domain name – this can be any of a tenant's verified domain 
        //public const string tenant = "graphdir1.onMicrosoft.com";

        //// Config for OAuth client credentials (2-legged OAuth, S2S Authn flow)
        //public const string clientId = "118473c2-7619-46e3-a8e4-6da8d5f56e12";
        //public const string clientSecret = "hOrJ0r0TZ4GQ3obp+vk3FZ7JBVP+TX353kNo6QwNq7Q=";

        //// Config for OAuth Authorization Code grant flow (3-legged OAuth Auth flow - includes user authentication)
        //public const string clientIdForUserAuthn = "66133929-66a4-4edc-aaee-13b04b03207d";  //for user_impersonation
        //public const string redirectUri = "https://localhost";

    }
}

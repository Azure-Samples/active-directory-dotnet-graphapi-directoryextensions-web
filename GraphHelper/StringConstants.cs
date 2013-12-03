using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphHelper
{
    public class StringConstants
    {
        // hardcoded access token for running against NOVA
        public const string AccessToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjBHUkJ0TVdGaG83Xy1xQmVNU0E1VlBVNXBjTSJ9.eyJhdWQiOiIwMDAwMDAwMi0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiIwMDAwMDAwMS0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDBAMDg4NGVhMDYtYzAyNy00ZTM5LWJiYmMtMTVjZmE0M2ZmNTljIiwibmJmIjoiMTM4NTY3NjYxOSIsImV4cCI6IjEzODgyNjg2MTkiLCJvaWQiOiJmZDU4YTgxNy1mMzgwLTRmZmQtOTk3My00NzMzNTU3NGY0NGMiLCJ0aWQiOiIwODg0ZWEwNi1jMDI3LTRlMzktYmJiYy0xNWNmYTQzZmY1OWMifQ.DUU9gNhJ3_h5enHWW5ZL8APWDBuu1mUmoXFU8CvQcO2ytaMLk6KAGvFbPy-8mFdaak-MQ-yZ9jqoKYtAEDR2D_nqPwJGGQn32Krg3gtEwLIUCxSTX3-RShiTIWH9t0b4xwvn3Q8XjT1CQWrKvXhb_4kY3MNno4xeOt9EOlSEdynWgTdyKAmTSsoQMtmff0Jj6bUB-VbMM76WG04FlrAuFTc-6M09uPCaqp-qU8cYQFO5oyNHgfKRMpEjEZYauulECO_i3PHR4_K6EEQkBXG0kd9Zy_nJBFYYm3kiMaGPK8YKfYaTkT7hOyPVEwrr0FY1LODKODx3dsJkx874c5KgcQ";

        //application principal id
        //public const string GraphPrincipalId = "https://graph.windows.net";

        // resource that is used to request a token for
        public const string FAKEresource = "https://graph.windows.net";
        public const string resource = "https://directory.27.MSODS.msol-nova.com";

        //this is the STS endpoint for Windows Azure AD
        public const string FAKEauthenticationEndpoint = "https://login.windows.net/";
        public const string authenticationEndpoint = "https://27.login.ORGID.msol-nova.com/";
        
        // The URL of Graph service endpoint 
        public const string FAKEbaseGraphUri = "https://graph.windows.net/";
        public const string baseGraphUri = "https://directory.27.MSODS.msol-nova.com/";

        // Graph API version
        public const string FAKEapiVersion = "api-version=2013-04-05";
        public const string apiVersion = "api-version=1.21-preview";
        
        // Graph API preview version
        public const string apiVersionPreview = "api-version=2013-04-10-preview";

        // Next section is tenant and application specific configuraiton.
        
        // Your tenant objectId or tenant domain name – this can be any of a tenant's verified domain 
        public const string FAKEtenant = "msonlinesetup.onmicrosoft.com";
        public const string tenant = "msonlinesetup.27.msods.msol-nova.com";

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

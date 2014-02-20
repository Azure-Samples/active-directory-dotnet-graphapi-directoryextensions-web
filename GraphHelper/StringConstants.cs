using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphHelper
{
    public class StringConstants
    {

        // resource that is used to request a token for
        public const string resource = "https://graph.windows.net";

        //this is the STS endpoint for Windows Azure AD
        public const string authenticationEndpoint = "https://login.windows.net/";

        // The URL of Graph service endpoint 
        public const string baseGraphUri = "https://graph.windows.net/";

        // Graph API version
        public const string apiVersion = "api-version=1.21-preview";

        // Graph API preview version
        public const string apiVersionPreview = "api-version=2013-04-10-preview";

        // Next section is tenant and application specific configuraiton.

        // Your tenant objectId or tenant domain name – this can be any of a tenant's verified domain
        public const string tenant = "msonlinesetup.onmicrosoft.com";

        // Config for OAuth client credentials (2-legged OAuth, S2S Authn flow)
        public const string clientId = "33b6cec1-7aa1-4574-9525-1f73a7c3b6e6";
        public const string AppObjectId = "821bdbf5-6927-4544-ab5e-e0506b456029";
        public const string clientSecret = "Kl/CdYAC32EmFxnC7Ek+hJHepi1hx9D9kHqxG5b0xVM=";

        // FIXTHIS: Config for OAuth Authorization Code grant flow (3-legged OAuth Auth flow - includes user authentication)
        //public const string clientIdForUserAuthn = "7318ca25-0f32-4d77-8843-2c4bc2a44326";  //for user_impersonation
        public const string redirectUri = "https://localhost";

        public const string extensionPropertyPrefix = "extension_";
        
        static public string getExtension(string strExtensionName)
        {
            string trioExtension = extensionPropertyPrefix;
            string strippedClientId = clientId.Replace("-", "");
            trioExtension += strippedClientId;
            trioExtension += "_";
            trioExtension += strExtensionName;
            return trioExtension;
        }
    }
}

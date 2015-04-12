using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphClient
{
    public class StringConstants
    {

        // resource that is used to request a token for
        public const string Resource = "https://graph.windows.net";

        //this is the STS endpoint for Windows Azure AD
        public const string AuthenticationEndpoint = "https://login.microsoftonline.com/";

        // The URL of Graph service endpoint 
        public const string BaseGraphUri = "https://graph.windows.net/";

        // Graph API version
        public const string ApiVersion = "api-version=1.21-preview";

        // Graph API preview version
        public const string ApiVersionPreview = "api-version=1.21-preview";

        // Next section is tenant and application specific configuraiton.

        // Your tenant objectId or tenant domain name – this can be any of a tenant's verified domain
        public static string Tenant = "dxtest.onmicrosoft.com";

        // Config for OAuth client credentials (2-legged OAuth, S2S Authn flow)
        public static string ClientId = "eeed6b47-a902-4c40-94ae-76d30e26a5ef";
        public static string AppObjectId = "a4eff3a2-2a90-494a-9db4-497958a0319f";
        public static string ClientSecret = "WzeIRPCwE54thByEV1a8+uambXJkuCRB1xH/fcGhOmM=";

        // FIXTHIS: Config for OAuth Authorization Code grant flow (3-legged OAuth Auth flow - includes user authentication)
        //public const string clientIdForUserAuthn = "7318ca25-0f32-4d77-8843-2c4bc2a44326";  //for user_impersonation
        public const string RedirectUri = "https://localhost";

        public const string LogFile = "AADGraph.log";
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Web;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

// for Graph API version 1.0, api-version=2013-04-05

namespace Microsoft.WindowsAzure.ActiveDirectory.GraphClient
{

    [DataContract]
    public class AadContacts
    {
        [DataMember(Name = "odata.metadata")] public string ODataMetadata { get; set; }
        [DataMember(Name = "value")] public List<AadContact> contact { get; set; } //collection
        [DataMember(Name = "odata.nextLink")] public string ODataNextLink { get; set; }
    }

    [DataContract]
    public class AadContact
    {
        [DataMember(Name = "odata.type")]  public string ODataType { get; set; }
        [DataMember] public string objectId { get; set; }
        [DataMember] public string objectType { get; set; }
        [DataMember] public string city { get; set; } //" Type="Edm.String" />
        [DataMember] public string country { get; set; } //" Type="Edm.String" />
        [DataMember] public string department { get; set; } //" Type="Edm.String" />
        [DataMember] public bool? dirSyncEnabled { get; set; } //" Type="Edm.Boolean" />
        [DataMember] public string displayName { get; set; }
        [DataMember] public string facsimileTelephoneNumber { get; set; } //" Type="Edm.String" />
        [DataMember] public string givenName { get; set; } //" Type="Edm.String" />
        [DataMember] public string jobTitle { get; set; } //" Type="Edm.String" />
        [DataMember] public string lastDirSyncTime { get; set; } //" Type="Edm.DateTime" />
        [DataMember] public string mail { get; set; } //" Type="Edm.String" />
        [DataMember] public string mailNickname { get; set; } //" Type="Edm.String" />
        [DataMember] public string mobile { get; set; } //" Type="Edm.String" />
        [DataMember] public string physicalDeliveryOfficeName { get; set; } //" Type="Edm.String" />
        [DataMember] public string postalCode { get; set; } //" Type="Edm.String" />
        [DataMember] public string[] proxyAddresses { get; internal set; } //" Type="Collection(Edm.String)" Nullable="false" />
        [DataMember] public List<provisioningError> provisioningErrors { get; set; } //" Type="Collection(Microsoft.WindowsAzure.ActiveDirectory.ProvisioningError)" Nullable="false" />
        [DataMember] public string state { get; set; } //" Type="Edm.String" />
        [DataMember] public string streetAddress { get; set; } //" Type="Edm.String" />
        [DataMember] public string surname { get; set; } //" Type="Edm.String" />
        [DataMember] public string telephoneNumber { get; set; } //" Type="Edm.String" />
    }

    [DataContract]
    public class AadUsers
    {
        [DataMember(Name="odata.metadata")] public string ODataMetadata  { get; set; }
        [DataMember(Name="value")] public List<AadUser> user { get; set; } //collection
        [DataMember(Name="odata.nextLink")] public string ODataNextLink { get; set; } 
    }

    [DataContract]
    public class JUsers
    {
        [DataMember(Name = "odata.metadata")]
        public string ODataMetadata { get; set; }
        [DataMember(Name = "value")]
        public List<JObject> users { get; set; } //collection
        [DataMember(Name = "odata.nextLink")]
        public string ODataNextLink { get; set; }
    }

    [DataContract]
    public class AadUser : AadContact
    {
        [DataMember] public string userPrincipalName { get; set; }
        [DataMember] public bool accountEnabled { get; set; }  //boolean
        [DataMember] public List<assignedLicense> assignedLicenses { get; set; } //collection
        [DataMember] public List<assignedPlan> assignedPlans { get; internal set; } //type="Collection(Microsoft.WindowsAzure.ActiveDirectory.AssignedPlan)" Nullable="false" />
        [DataMember] public string[] otherMails { get; set; } //" Type="Collection(Edm.String)" Nullable="false" />
        [DataMember] public string passwordPolicies { get; set; } //" Type="Edm.String" />
        [DataMember] public passwordProfile passwordProfile { get; set; } //" Type="Microsoft.WindowsAzure.ActiveDirectory.PasswordProfile" />
        [DataMember] public string preferredLanguage { get; set; } //" Type="Edm.String" />
        [DataMember] public List<provisionedPlan> provisionedPlans { get; internal set; } //" Type="Collection(Microsoft.WindowsAzure.ActiveDirectory.ProvisionedPlan)" Nullable="false" />
        [DataMember] public string usageLocation { get; set; } //" Type="Edm.String" />
    }

    [DataContract]
    public class AadGroups
    {
        [DataMember(Name = "odata.metadata")] public string ODataMetadata { get; set; }
        [DataMember(Name = "value")] public List<AadGroup> group { get; set; } //collection
        [DataMember(Name = "odata.nextLink")] public string ODataNextLink { get; set; }
    }

    [DataContract]
    public class AadGroup : AadContact
    {
        // add unique group properties 
        [DataMember] public string description { get; set; }
        [DataMember] public bool securityEnabled { get; set; }
        [DataMember] public bool mailEnabled { get; set; }

    }

    [DataContract]
    public class AadObjects
    {
        [DataMember(Name = "odata.metadata")] public string ODataMetadata { get; set; }
        [DataMember(Name = "value")] public List<AadObject> aadObject { get; set; } //collection
        [DataMember(Name = "odata.nextLink")] public string ODataNextLink { get; set; }
    }


    // AadObject can be a User,Group or Contact Object
    // Therefore, we inherit from the user object (which inherits from contact object)
    // then add Group specific properties.
    [DataContract]
    public class AadObject : AadUser
    {
        [DataMember] public string description { get; set; }
        [DataMember] public bool securityEnabled { get; set; }
        [DataMember] public bool mailEnabled { get; set; }
    }


      [DataContract]
    public class AadRoles
    {
        [DataMember(Name = "odata.metadata")] public string ODataMetadata { get; set; }
        [DataMember(Name = "value")] public List<AadRole> role { get; set; } //collection
    }

    [DataContract]
    public class AadRole
    {
        [DataMember(Name = "odata.type")]  public string ODataType { get; set; }
        [DataMember]  public string objectId { get; set; }
        [DataMember]  public string objectType { get; set; }
        [DataMember]  public string description { get; set; }
        [DataMember]  public string displayName { get; set; }
        [DataMember]  public bool isSystem { get; set; }
        [DataMember]  public bool roleDisabled { get; set; }
    }




    [DataContract]
    public class assignedLicense
    {
        [DataMember] public Guid skuId { get; set; }
        [DataMember] public string[] disabledPlans {get; set;}
    }

    [DataContract]
    public class userLicense
    {

        [DataMember]
        public List<addLicense> addLicenses { get; set; }
        [DataMember]
        public string[] removeLicenses { get; set; }
        // public List<removeLicense> removeLicenses { get; set; }
    }

    [DataContract]
    public class addLicense
    {
        [DataMember]
        public string[] disabledPlans { get; set; }
        [DataMember]
        public string skuId { get; set; }
    }

    [DataContract]
    public class removeLicense
    {
        [DataMember]
        public string[] skuId { get; set; }
    }

    [DataContract]
    public class assignedPlan
    {
        [DataMember] public string assignedTimestamp { get; set; }
        [DataMember] public string capabilityStatus { get; set; }
        [DataMember] public string service { get; set; }
        [DataMember] public Guid servicePlanId { get; set; }

    }

    [DataContract]
    public class provisionedPlan
    {
        [DataMember] public string capabilityStatus { get; set; }   
        [DataMember] public string provisioningStatus { get; set; }
        [DataMember] public string service { get; set; }
    }

    [DataContract]
    public class provisioningError
    {
        [DataMember] public string errorDetail { get; set; }
        [DataMember] public bool resolved { get; set; }
        [DataMember] public string service { get; set; }
        [DataMember]
        public string timestamp { get; set; }  //" Type="Edm.DateTime" />
    }

    [DataContract]
    public class passwordProfile
    {
        [DataMember] public string password { get; set; }
        [DataMember] public bool forceChangePasswordNextLogin { get; set; }
    }

    [DataContract]
    public class AadTenantDetails
    {
        [DataMember(Name = "odata.metadata")]
        public string ODataMetadata { get; set; }
        [DataMember(Name = "value")] public List<AadTenantDetail> tenant { get; set; } //collection
    }

    [DataContract]
    public class AadTenantDetail
    {
            [DataMember(Name = "odata.type")] public string ODataType { get; set; }
            [DataMember] public string objectId { get; set; }
            [DataMember] public string objectType { get; set; }
            [DataMember] public string city { get; set; } //" Type="Edm.String" />
            [DataMember] public string country { get; set; } //" Type="Edm.String" />
            [DataMember] public string companyLastDirSyncTime { get; set; } //" Type="Edm.DateTime" />
            [DataMember] public bool? dirSyncEnabled { get; set; } //" Type="Edm.Boolean" />
            [DataMember] public string displayName { get; set; }
            [DataMember] public string countryLetterCode { get; set; } //" Type="Edm.String" />
            [DataMember] public string preferredLanguage { get; set; } //" Type="Edm.String" />
            [DataMember] public string state { get; set; } //" Type="Edm.String" />
            [DataMember] public string street { get; set; } //" Type="Edm.String" />
            [DataMember] public string postalCode { get; set; } //" Type="Edm.String" />
            [DataMember] public string telephoneNumber { get; set; } //" Type="Edm.String" /
            [DataMember] public string tenantType { get; set; }
            [DataMember] public List<provisionedPlan> provisionPlans { get; set; } //" Type="Collection(Microsoft.WindowsAzure.ActiveDirectory.ProvisionedPlan)" Nullable="false" />
            [DataMember] public List<provisioningError> provisioningErrors { get; set; }
            [DataMember] public List<assignedPlan> assignedPlans { get; set; } //" Type="Collection(Microsoft.WindowsAzure.ActiveDirectory.ProvisioningError)" Nullable="false" />
            [DataMember] public string[] marketingNotificationEmails { get; set; }
            [DataMember] public string[] technicalNotificationMails { get; set; }
            [DataMember] public List<verifiedDomain> verifiedDomains { get; set; }
    }

    [DataContract]
    public class verifiedDomain
    {
        [DataMember] public string capabilities {get;set;}
        [DataMember(Name="default")] public Boolean dfault {get; set;}
        [DataMember] public string name {get;set;}
        [DataMember] public string type {get;set;}
        [DataMember] public string id { get; set; }
        [DataMember] public Boolean initial {get;set;}     
    }

    [DataContract]
    public class AadSubscribedSkus
    {
        [DataMember(Name = "odata.type")] public string ODataType { get; set; }
        [DataMember(Name = "value")]
        public List<AadSubscribedSku> subscribedSku { get; set; } //collection
    }

    [DataContract]
    public class AadSubscribedSku
    {
        [DataMember] public string capabilityStatus { get; set; }
        [DataMember] public int consumedUnits { get; set; }
        [DataMember] public string objectId { get; set; }
        [DataMember(Name="prepaidUnits")] public prePaidUnits prepaidUnits { get; set; }
        [DataMember] public Guid skuId { get; set; }
        [DataMember] public string skuPartNumber { get; set; }
        [DataMember] public List<servicePlan> servicePlans { get; set; }
    }

    [DataContract]
    public class prePaidUnits
    {
        [DataMember] public int enabled { get; set; }
        [DataMember] public int suspended { get; set; }
        [DataMember] public int warning { get; set; }
    }

    [DataContract]
    public class servicePlan
    {
        [DataMember] public Guid servicePlanId { get; set; }
        [DataMember] public string servicePlanName { get; set; }
    }

    public class urlLink
    {
        public string url { get; set; }

    }

    public class isMemberOfResult
    {
        public bool value { get; set; }
    }

    [DataContract]
    public class MemberGroupsData
    {
    [DataMember(Name="odata.metadata")] public string ODataMetadata  { get; set; }
    [DataMember(Name = "value")]
        public string[] value { get; set; } //collection
    }

    [DataContract]
    public class GroupIds
    {
        [DataMember]
        public string[] groupIds { get; set; }
    }

    [DataContract]
    public class AadApplications
    {
        [DataMember(Name = "odata.metadata")]
        public string ODataMetadata { get; set; }
        [DataMember(Name = "value")]
        public List<AadApplication> application { get; set; } //collection
        [DataMember(Name = "odata.nextLink")]
        public string ODataNextLink { get; set; }
    }

    [DataContract]
    public class AadApplication
    {
        [DataMember(Name = "odata.type")] public string ODataType { get; set; }
        [DataMember] public string objectType { get; set; }
        [DataMember] public string objectId { get; set; }
        [DataMember] public string appId {get; set;} // Type="Edm.Guid" />
        [DataMember] public bool availableToOtherTenants {get; set;} // Type="Edm.Boolean" />
        [DataMember] public string displayName {get; set;} // Type="Edm.String" />
        [DataMember] public string errorUrl {get; set;} // Type="Edm.String" />
        [DataMember] public string homepage {get; set;} // Type="Edm.String" />
        [DataMember] public string[] identifierUris {get; set;} // Type="Collection(Edm.String)" Nullable="false" />
        [DataMember] public List<KeyCredential> keyCredentials {get; set;} // Type="Collection(Microsoft.WindowsAzure.ActiveDirectory.KeyCredential)" Nullable="false" />
        [DataMember] public Stream mainLogo {get; set;} // Type="Edm.Stream" Nullable="false" />
        [DataMember] public string logoutUrl {get; set;} // Type="Edm.String" />
        [DataMember] public List<PasswordCredential> passwordCredentials {get; set;} // Type="Collection(Microsoft.WindowsAzure.ActiveDirectory.PasswordCredential)" Nullable="false" />
        [DataMember] public bool publicClient {get; set;} // Type="Edm.Boolean" />
        [DataMember] public string[] replyUrls {get; set;} // Type="Collection(Edm.String)" Nullable="false" />
        [DataMember] public string samlMetadataUrl {get; set;} // Type="Edm.String" />

    }

    [DataContract]
    public class AadServicePrincipals
    {
        [DataMember(Name = "odata.metadata")]
        public string ODataMetadata { get; set; }
        [DataMember(Name = "value")] public List<AadServicePrincipal> servicePrincipal { get; set; } //collection
        [DataMember(Name = "odata.nextLink")]
        public string ODataNextLink { get; set; }
    }

    [DataContract]
    public class AadServicePrincipal
    {
        [DataMember(Name = "odata.type")] public string ODataType { get; set; }
        [DataMember] public string objectType { get; set; }
        [DataMember] public string objectId { get; set; }
        [DataMember] public bool accountEnabled {get; set;}  //"Edm.Boolean" />
        [DataMember] public string appId {get; set;}  //"Edm.Guid" />
        [DataMember] public string displayName {get; set;}  //"Edm.String" />
        [DataMember] public string errorUrl {get; set;}  //"Edm.String" />
        [DataMember] public string homepage {get; set;}  //"Edm.String" />
        [DataMember] public List<KeyCredential> keyCredentials {get; set;}  //"Collection(Microsoft.WindowsAzure.ActiveDirectory.KeyCredential)" Nullable="false" />
        [DataMember] public string logoutUrl {get; set;}  //"Edm.String" />
        [DataMember] public List<PasswordCredential> passwordCredentials {get; set;}  //"Collection(Microsoft.WindowsAzure.ActiveDirectory.PasswordCredential)" Nullable="false" />
        [DataMember] public string publisherName {get; set;}  //"Edm.String" />
        [DataMember] public string[] replyUrls {get; set;}  //"Collection(Edm.String)" Nullable="false" />
        [DataMember] public string samlMetadataUrl {get; set;}  //"Edm.String" />
        [DataMember] public string[] servicePrincipalNames {get; set;}  //"Collection(Edm.String)" Nullable="false" />
        [DataMember] public string[] tags {get; set;}  //"Collection(Edm.String)" Nullable="false" />
    }

    [DataContract]
    public class KeyCredential
    {

        [DataMember] public string customKeyIdentifier {get;set;} // Type="Edm.Binary" />
        [DataMember] public string endDate {get;set;} // Type="Edm.DateTime" />
        [DataMember] public string keyId {get;set;} // Type="Edm.Guid" />
        [DataMember] public string startDate {get;set;} // Type="Edm.DateTime" />
        [DataMember] public string type {get;set;} // Type="Edm.String" />
        [DataMember] public string usage {get;set;} // Type="Edm.String" />
        [DataMember] public string value {get;set;} // Type="Edm.Binary" />
    }

    [DataContract]
    public class PasswordCredential
    {
            [DataMember] public string customKeyIdentifier {get;set;} // Type="Edm.Binary" />
            [DataMember] public string endDate {get;set;} // Type="Edm.DateTime" />
            [DataMember] public string keyId {get;set;} // Type="Edm.Guid" />
            [DataMember] public string startDate {get;set;} // Type="Edm.DateTime" />
            [DataMember] public string value {get;set;} // Type="Edm.String" />
    }

    public class ExtensionDefinition
    {
        public string name { get; set; }
        public string dataType { get; set; }
        private IList<string> targetClasses = new List<string>();
        public IList<string> targetObjects
        {
            get
            {
                return targetClasses;
            }
        }
    }

    [DataContract]
    public class ODataError
    {
        [DataMember(Name = "odata.error")] public Error error { get; set; }
    }

    [DataContract]
    public class Error
    {
        [DataMember] public string code { get; set; }
        [DataMember] public Message message { get; set; }
    }

    [DataContract]
    public class Message
    {
        [DataMember] public string lang { get; set; }
        [DataMember] public string value { get; set; }
    }
}

Goals
When we started working on directory extensions, we had two goals:
1.	Enable organizations to move their applications to the cloud. Seamlessly synchronizing on-premises schema extensions to Azure AD will allow organizations to leverage investments in on-premises applications as they move to the cloud.
2.	Enable ISVs to build more powerful directory-aware applications. Allowing application developers to extend the directory allows them to develop richer directory-aware applications without worrying about access controls, availability requirements, etc. of a user profile store.
 
The first goal really brings on-premises AD and Azure AD together, which we know is hugely important to you. However, the second goal is a prerequisite for the first; therefore, that is where we started. This preview provides REST interfaces for registering, reading, writing, and filtering by extension values.
 
Application-Centric Model
Since most people think of directory extensions as belonging to the tenant, let me take a moment to explain our application-centric approach. You may want to read this post to understand how to write an application to access the Graph API. To enable an application to register extensions (goal #2), the extensions are registered on the Application object in the directory and referenced from all the tenants consenting to that Application. Once a customer tenant has consented to an Application (even for read) the extensions registered on that Application are available in the consenting tenant for reading/writing by any Application that has the appropriate access. If the app developer wants to add more extension attributes, she can update her Application (in her developer tenant) and any tenants that are currently consented to this Application will instantly be enabled for the new attributes. If consent is removed, if the extension is deleted, or if the Application is deleted, the extension values will no longer be accessible (and cleaned up as a background task once we implement garbage collection) on the corresponding directory objects.
 
Types and Limitations
Currently “User”, “Group”, “TenantDetail”, “Device”, “Application” and “ServicePrincipal” entities can be extended with “String” type or “Binary” type single-valued attributes. String type extensions can have maximum of 256 characters and binary extensions are limited to 256 bytes. 100 extension values (across ALL types and ALL applications) can be written to any single object. Prefix searches on extensions are limited to 71 characters for string searches and 207 bytes for searches on binary extensions.
 
Preparing your Application to Register Directory Extensions
When your Application is consented to read or write in your tenant, a service principal is created in your tenant. In order to register directory extensions, your this service principal must be a member of the “Company Administrator” role. In order to add your Application service principal to the “Company Administrator” role the following PowerShell commands may be helpful:
Get-MsolServicePrincipal – to retrieve the ObjectId for the service principal corresponding to your Application
Get-MsolRole – to retrieve the ObjectId for your “Company Administrator” role in your tenant
Add-MsolRoleMember -RoleObjectId <Company Administrator ObjectId> -RoleMemberObjectId <Service Principal ObjectId> -RoleMemberType ServicePrincipal
Get-MsolRoleMember -RoleObjectId <Company Administrator ObjectId> to confirm you have successfully added your service principal to the “Company Administrator” role

Registering an Extension
Let’s walk through an example. Contoso has built an OrgChart application and wants to allow users to make Skype calls from it. AAD does not expose a SkypeID user property. The OrgChart developer could use a separate store such as SQL Azure to store a record for each user’s SkypeID. Instead, the developer registers a String extension on the User object in his tenant. He does this by creating an “extensionProperty” on the Application using Graph API.
POST https://graph.windows.net/contoso.com/applications/<applicationObjectID>/extensionProperties?api-version=1.21-preview 
{
“name”: “skypeId”,
“dataType”: “String”,
“targetObjects”: [“User”]
}
If the operation is successful, it will return 201 along with the fully qualified extension property name to be used for updating the intended types.
201 Created
{
“objectId”: “5ea3a29b-8efd-46bf-9dc7-f226e839d146”,
“objectType”: “ExtensionProperty”,
“name”: “extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”,
“dataType”: “String”,         
“targetObjects”: [“User”]
}
 
Viewing Directory Extensions Registered by your Application
You can view extensions registered by your application by issuing a GET of the extension properties of the application. This will provide object ID, data type, and target objects for each extension registered by the application.
GET https://graph.windows.net/contoso.com/applications/<applicationObjectID>/extensionProperties?api-version=1.21-preview 

Unregistering an Extension
You can unregister an extension registered by your application by issuing a DELETE of the extension object ID as follows:
DELETE https://graph.windows.net/contoso.com/applications/<applicationObjectID>/extensionProperties/<extensionObjectID>?api-version=1.21-preview 

Writing Extension Values
Once this application is consented by the admin, any user in the tenant can be updated to include this new property. For example,
PATCH https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview 
{
“extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”: “joe.smith”
}
The server will return a 204 if user was successfully updated. The extension value can be removed by sending the same PATCH request with “null” value.
PATCH https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview 
{
“extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”: null
}
 
Reading Extension Values
When directory objects are retrieved, they automatically include the extension values. For example:
GET https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview 
200 OK
{
“objectId”: “ff7cd54a-84e8-4b48-ac5a-21abdbaef321”,
“displayName”: “Joe Smith”,
“userPrincipalName”: “joe@contoso.com“,
“objectType”: “User”,
“mail”: “null”,
“accountEnabled”: “True” ,
“extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”: “joe.smith”
}
 
Filtering by Extension Values
The extension values can also be used as a part of $filter to search directory similar to any existing property. For example:
GET https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview&$filter=extension_d8dde29f1095422e91537a6cb22a2f74_skypeId+eq+'joe.smith'

Sample Code
We have published a couple of samples to GitHub to showcase and illustrate the use of directory extensions. We plan to enhance them based on your feedback and as the feature evolves. 

PHP Sample
https://github.com/WindowsAzureAD/WindowsAzureAD-GraphAPI-Sample-PHP

OrgChart Sample
https://github.com/WindowsAzureAD/WindowsAzureAD-GraphAPI-Sample-OrgChart
This app allows reading extension values in a sample tenant out of the box. If provides credentials to your Application, this app provides UI to register and write extension attributes as well.
You can focus on the DirectoryExtensions class in Models\DirectoryExtensions.cs as these methods focus on working with extension attributes:
	RegisterExtension - registers a user extension with given name on Application specified by StringConstants.AppObjectId
	CreateUser - creates users with extensions, calls set user to set manager
	GetUser - gets user with extensions
	SetUser - sets extension and manager attribute on a user
	SetUserManager - sets user's manager attribute
	GetUsersManager - get user's manager with extensions
	GetUsersDirectReports - get user's direct reports with extensions
	GetExtensionName - gets extension name based on app id
	GetExtensionRegistryUserUpn - username of user where "registered" value is set on all registered extensions
	GetExtensionRegistryUser - user where "registered" value is set on all registered extensions
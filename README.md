---
services: active-directory
platforms: dotnet
author: dstrockis
---

# Extending the Azure AD directory schema with custom properties


## Goals
When we started working on directory extensions, we had two goals:

1. Enable ISVs to build more powerful directory-aware applications. Allowing application developers to extend the directory allows them to develop richer directory-aware applications without worrying about access controls, availability requirements, etc. of a user profile store.
2. Enable organizations to move their applications to the cloud. Seamlessly synchronizing on-premises schema extensions to Azure AD will allow organizations to leverage investments in on-premises applications as they move to the cloud.

This first preview focuses on goal #1 above. This preview provides REST interfaces for an application to register, read, write, and filter by extension values. We have already started prototyping and designing integration with our DirSync and FIM tools to enable easy synchronization of custom schema extensions between on-premises AD and Azure AD. So it won't be very long before we can turn on features that enable the second goal as well.

## Application-Centric Model
Since most people think of directory extensions as belonging to the tenant, let me take a moment to explain our application-centric approach. You may want to read [this post](http://msdn.microsoft.com/en-us/library/windowsazure/dn151791.aspx) to understand how to write an application to access the Graph API. To enable an application to register extensions, the extensions are registered on the Application object in the directory and referenced from all the tenants consenting to that Application. Once a customer tenant has consented to an Application (even for read) the extensions registered on that Application are available in the consenting tenant for reading/writing by any Application that has the appropriate access. If the app developer wants to add more extension attributes, she can update her Application (in her developer tenant) and any tenants that are currently consented to this Application will instantly be enabled for the new attributes. If consent is removed, if the extension is deleted, or if the Application is deleted, the extension values will no longer be accessible (and cleaned up as a background task once we implement garbage collection) on the corresponding directory objects.

## Types and Limitations
Currently “User”, “Group”, “TenantDetail”, “Device”, “Application” and “ServicePrincipal” entities can be extended with “String” type or “Binary” type single-valued attributes. String type extensions can have maximum of 256 characters and binary extensions are limited to 256 bytes. 100 extension values (across ALL types and ALL applications) can be written to any single object. Prefix searches on extensions are limited to 71 characters for string searches and 207 bytes for searches on binary extensions.

## Registering an Extension
Let’s walk through an example. Contoso has built an OrgChart application and wants to allow users to make Skype calls from it. AAD does not expose a SkypeID user property. The OrgChart developer could use a separate store such as SQL Azure to store a record for each user’s SkypeID. Instead, the developer registers a String extension on the User object in his tenant. He does this by creating an “extensionProperty” on the Application using Graph API.
```
POST https://graph.windows.net/contoso.com/applications/<applicationObjectID>/extensionProperties?api-version=1.21-preview 
{
“name”: “skypeId”,
“dataType”: “String”,
“targetObjects”: [“User”]
}
```
If the operation is successful, it will return 201 along with the fully qualified extension property name to be used for updating the intended types.
```
201 Created
{
“objectId”: “5ea3a29b-8efd-46bf-9dc7-f226e839d146”,
“objectType”: “ExtensionProperty”,
“name”: “extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”,
“dataType”: “String”,         
“targetObjects”: [“User”]
}
```
## Viewing Directory Extensions Registered by your Application
You can view extensions registered by your application by issuing a GET of the extension properties of the application. This will provide object ID, data type, and target objects for each extension registered by the application.
```
GET https://graph.windows.net/contoso.com/applications/<applicationObjectID>/extensionProperties?api-version=1.21-preview 
```
## Unregistering an Extension
You can unregister an extension registered by your application by issuing a DELETE of the extension object ID as follows:
```
DELETE https://graph.windows.net/contoso.com/applications/<applicationObjectID>/extensionProperties/<extensionObjectID>?api-version=1.21-preview 
```
## Writing Extension Values
Once this application is consented by the admin, any user in the tenant can be updated to include this new property. For example,
```
PATCH https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview 
{
“extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”: “joe.smith”
}
```
The server will return a 204 if user was successfully updated. The extension value can be removed by sending the same PATCH request with “null” value.
```
PATCH https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview 
{
“extension_d8dde29f1095422e91537a6cb22a2f74_skypeId”: null
}
```
## Reading Extension Values
When directory objects are retrieved, they automatically include the extension values. For example:
```
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
```
## Filtering by Extension Values
The extension values can also be used as a part of $filter to search directory similar to any existing property. For example:
```
GET https://graph.windows.net/contoso.com/users/joe@contoso.com?api-version=1.21-preview&$filter=extension_d8dde29f1095422e91537a6cb22a2f74_skypeId+eq+'joe.smith'
```
## Sample Code
We have published a couple of samples to GitHub to showcase and illustrate the use of directory extensions. We plan to enhance them based on your feedback and as the feature evolves.
### PHP Sample
https://github.com/WindowsAzureAD/WindowsAzureAD-GraphAPI-Sample-PHP
### OrgChart Sample
https://github.com/WindowsAzureAD/WindowsAzureAD-GraphAPI-Sample-OrgChart
This app allows reading extension values in a sample tenant (dxtest.onmicrosoft.com) out of the box.
This app is designed to register new extension attributes on one Application (the current Application) at a time. 
This app allows registering and writing extension attributes to a tenant if you provide credentials to an Application consented for write in that tenant:
* AppId: You can retrieve this from the "Application ID" textbox in the Properties section of the Settings menu in the Azure Portal.
* AppSecret: You can retrieve this from the "Keys" section of the Settings menu in the Azure Portal.
* AppObjectId: Retrieve from "objectid" field in GraphExplorer (https://graphexplorer.cloudapp.net/) by navigating to Resource: https://graph.windows.net/any_verified_domain/applications.
* AppTenant: Any verified domain for the tenant owning the Application.

This app will use the following string extension values if they are present on users:
* trio: users marked with a trio extension attribute will be grouped by that trio name at each level of the orgchart
* skype: users marked with a skype extension attribute will present UI for making a skype call to the referenced skype username

You can focus on the DirectoryExtensions class in Models\DirectoryExtensions.cs as these methods focus on working with extension attributes:
* RegisterExtension - registers a user extension with given name on Application specified by StringConstants.AppObjectId
* CreateUser - creates users with extensions, calls set user to set manager
* GetUser - gets user with extensions
* SetUser - sets extension and manager attribute on a user
* SetUserManager - sets user's manager attribute
* GetUsersManager - get user's manager with extensions
* GetUsersDirectReports - get user's direct reports with extensions
* GetExtensionName - gets extension name based on app id
* GetExtensionRegistryUserUpn - username of user where "registered" value is set on all registered extensions
* GetExtensionRegistryUser - user where "registered" value is set on all registered extensions

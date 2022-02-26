# Securing ASP.NET Core 3 with OAuth 2 and OpenID Connect

Fully functioning finished sample code for my Securing ASP.NET Core 3 with OAuth2 and OpenID Connect course. Make sure you start up / deploy the IDP, Client & API project when running the finished solution.

The accompanying course can be watched here: https://app.pluralsight.com/library/courses/securing-aspnet-core-3-oauth2-openid-connect/



## Prepare

IdenityServer4 documentation:
https://identityserver4.readthedocs.io/en/latest/topics/signin_external_providers.html

### install dotnet project template for IdentityServer4

PS C:\YUHECHEN\AspNetCore\AspNetCore.OAuth2\src> dotnet new -i IdentityServer4.Templates
The following template packages will be installed:
   IdentityServer4.Templates

Success: IdentityServer4.Templates::4.0.1 installed the following templates:
Template Name                                         Short Name  Language  Tags
----------------------------------------------------  ----------  --------  -------------------
IdentityServer4 Empty                                 is4empty    [C#]      Web/IdentityServer4
IdentityServer4 Quickstart UI (UI assets only)        is4ui       [C#]      Web/IdentityServer4
IdentityServer4 with AdminUI                          is4admin    [C#]      Web/IdentityServer4
IdentityServer4 with ASP.NET Core Identity            is4aspid    [C#]      Web/IdentityServer4
IdentityServer4 with Entity Framework Stores          is4ef       [C#]      Web/IdentityServer4
IdentityServer4 with In-Memory Stores and Test Users  is4inmem    [C#]      Web/IdentityServer4

### create an empty IdentityServer4 project
PS C:\YUHECHEN\AspNetCore\AspNetCore.OAuth2\src> dotnet new is4empty -n Marvin.IDP
The template "IdentityServer4 Empty" was created successfully.

### run IdentiyServer and inspect discovery document (configuration)
The default url for the discovery document:
https://localhost:44318/.well-known/openid-configuration

Other application can read the discovery document to discover the enpoints.

### add UI to IdentityServer
PS C:\YUHECHEN\AspNetCore\AspNetCore.OAuth2\src\Marvin.IDP> dotnet new is4ui
The template "IdentityServer4 Quickstart UI (UI assets only)" was created successfully.

Uncomment disabled code in Startup.cs to add MVC and controllers. Then launch the the server again from VS.
Use the following url to access to the UI.
https://localhost:44318/

### call into UserInfo endpoint to get address
Install identitymodel nuget package which provides extention function to help call UserInfo endpoint.
Define a HttpClient service in Startup.cs.

### parse access token in ImageGallery.API and use it for authorization (RBAC).
In ImageGallery.API project, install IdentityServer4.AccessTokenValidation.
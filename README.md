# Securing ASP.NET Core 3 with OAuth 2 and OpenID Connect

Fully functioning finished sample code for my Securing ASP.NET Core 3 with OAuth2 and OpenID Connect course. Make sure you start up / deploy the IDP, Client & API project when running the finished solution.

The accompanying course can be watched here: https://app.pluralsight.com/library/courses/securing-aspnet-core-3-oauth2-openid-connect/



## Prepare

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



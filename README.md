# Securing ASP.NET Core 3 with OAuth 2 and OpenID Connect

Fully functioning finished sample code for my Securing ASP.NET Core 3 with OAuth2 and OpenID Connect course. Make sure you start up / deploy the IDP, Client & API project when running the finished solution.

The accompanying course can be watched here: https://app.pluralsight.com/library/courses/securing-aspnet-core-3-oauth2-openid-connect/



## Prepare

IdenityServer4 documentation:
https://identityserver4.readthedocs.io/en/latest/topics/signin_external_providers.html
https://github.com/IdentityServer/IdentityServer4/tree/main/samples

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

### authenticate user in ImageGallery.Client web server.
Install Microsoft.AspNetCore.Authentication.OpenIdConnect.
It's an ASP.NET Core middleware that enables an application to support the OpenID Connect authentication workflow.
Check Startup.cs for actual code.

### call into UserInfo endpoint to get address
Install identitymodel nuget package which provides extention function to help call UserInfo endpoint.
Define a HttpClient service in Startup.cs. Use it to request extra user profile info from UserInfo endpoint.

### parse access token in ImageGallery.API and use it for authorization (RBAC).
In ImageGallery.API project, install IdentityServer4.AccessTokenValidation.

### sample id token

eyJhbGciOiJSUzI1NiIsImtpZCI6IjhDNjk3N0E5QUUzRjRFODQ4RDBBRTI4N0ZEQjMyOUY3IiwidHlwIjoiSldUIn0.eyJuYmYiOjE2NDU0MTcwNjQsImV4cCI6MTY0NTQxNzM2NCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTgiLCJhdWQiOiJpbWFnZWdhbGxlcnljbGllbnQiLCJub25jZSI6IjYzNzgxMDEzODUyMTgxMTAzOS5OMlZrWTJWaE1HUXRORFF5TUMwME56a3pMVGhrTVdJdE5XTmlaV1ptT1RkalkyVXdNRGcwWVRrMFpqZ3RaVGxtTXkwMFkyWTJMVGxrTkRrdE0yVmlOREkxTVdKbU9UUXgiLCJpYXQiOjE2NDU0MTcwNjQsImF0X2hhc2giOiJEQmlXYnJXZkFxN2hwVGloaEdYenZBIiwic19oYXNoIjoiUHdTbjA0eWR1RTVsb1BKZzMwZVJqQSIsInNpZCI6IkE2MTQzOTBBNEEzNUE0RTZCMkZFQzQ2OUY4QjI4RTRFIiwic3ViIjoiZDg2MGVmY2EtMjJkOS00N2ZkLTgyNDktNzkxYmE2MWIwN2M3IiwiYXV0aF90aW1lIjoxNjQ1NDE3MDYwLCJpZHAiOiJsb2NhbCIsImFtciI6WyJwd2QiXX0.my319VxJ-DRZfi2mXIblEY8XpyYMunmhJx1BKQg3sC-n-dEvXGzUgmyW-pjvi8gjvm_ooHErosA1RXLc-4tq448fEXyMpYKVJhM-dKskZkHEApNJ8qodW7DVn30ARJhFI9RIfiTdxbkcQniJsUfnpk2TrB513LvyBEfC81mzapcJDbAYZv-r6zwy68Z_N6m1q6ktzDgyYLnTq4Bl6ek6qlOkKY5A8xF8WjTS-IARD2FqQvJ5Q-sLYnv-b0rbLsgxXSW8ymmcxczOF41Dxt28jhzDGylhq9dnccYwsPtNO1lZDqySBLg08MoMVdYMPx9crHzxb6WTD86sMUBn_dya9w

Decodes to :

HEADER:ALGORITHM & TOKEN TYPE

{
  "alg": "RS256",
  "kid": "8C6977A9AE3F4E848D0AE287FDB329F7",
  "typ": "JWT"
}
PAYLOAD:DATA

{
  "nbf": 1645417064,
  "exp": 1645417364,
  "iss": "https://localhost:44318",
  "aud": "imagegalleryclient",
  "nonce": "637810138521811039.N2VkY2VhMGQtNDQyMC00NzkzLThkMWItNWNiZWZmOTdjY2UwMDg0YTk0ZjgtZTlmMy00Y2Y2LTlkNDktM2ViNDI1MWJmOTQx",
  "iat": 1645417064,
  "at_hash": "DBiWbrWfAq7hpTihhGXzvA",
  "s_hash": "PwSn04yduE5loPJg30eRjA",
  "sid": "A614390A4A35A4E6B2FEC469F8B28E4E",
  "sub": "d860efca-22d9-47fd-8249-791ba61b07c7",
  "auth_time": 1645417060,
  "idp": "local",
  "amr": [
    "pwd"
  ]
}

### sample access token
eyJhbGciOiJSUzI1NiIsImtpZCI6IjhDNjk3N0E5QUUzRjRFODQ4RDBBRTI4N0ZEQjMyOUY3IiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDU4ODc4NjgsImV4cCI6MTY0NTg5MTQ2OCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTgiLCJhdWQiOlsiaW1hZ2VnYWxsZXJ5YXBpIiwiaHR0cHM6Ly9sb2NhbGhvc3Q6NDQzMTgvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6ImltYWdlZ2FsbGVyeWNsaWVudCIsInN1YiI6ImQ4NjBlZmNhLTIyZDktNDdmZC04MjQ5LTc5MWJhNjFiMDdjNyIsImF1dGhfdGltZSI6MTY0NTg4Nzg2NiwiaWRwIjoibG9jYWwiLCJyb2xlIjoiRnJlZVVzZXIiLCJqdGkiOiIyQUE2MzBCQjVCNjNGMzU3QzI2RDU3MUM2OTk0Qzc0QiIsInNpZCI6Ijc3NjZFRjQwQUZFNzM0REEyRDVGRDRFQkY0NzhCNDIxIiwiaWF0IjoxNjQ1ODg3ODY4LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiYWRkcmVzcyIsInJvbGVzIiwiaW1hZ2VnYWxsZXJ5YXBpIl0sImFtciI6WyJwd2QiXX0.Om68ITHraWJNcqSbsxfCphmtUouHyrLXBhZwFa5z1iFxNdhOYyBWiSqZuqziW4xq-QcYGR3f7fhL8n3SiPGJNF-qAwLIlSLln21p9Wi9RFJQ9JlCnn2PBbZ0PnRZZE7TLr5Fc7kRoaaAZR95mvohBsch5_GVzX8eAIBWpHT5NDgUYgXamUPGGqjkiVOtOAB-zX2gSyc2TQqgDN7vrtSBXiqe-gQDUOGnqo__yJvDG1KFnEaeEIqjsBpiOAEFto8jYQiYZ_FKcdM_eHYm1hfyHqwHz2QKfKBvnFidrSxX3fvdTdoow3yc5v5PLcUa2OpggRYd9zkUTMwA5DDwuVIHKg

Decodes to:
  "exp": 1645891468,
  "iss": "https://localhost:44318",
  "aud": [
    "imagegalleryapi",
    "https://localhost:44318/resources"
  ],
  "client_id": "imagegalleryclient",
  "sub": "d860efca-22d9-47fd-8249-791ba61b07c7",
  "auth_time": 1645887866,
  "idp": "local",
  "role": "FreeUser",
  "jti": "2AA630BB5B63F357C26D571C6994C74B",
  "sid": "7766EF40AFE734DA2D5FD4EBF478B421",
  "iat": 1645887868,
  "scope": [
    "openid",
    "profile",
    "address",
    "roles",
    "imagegalleryapi"
  ],
  "amr": [
    "pwd"
  ]
}



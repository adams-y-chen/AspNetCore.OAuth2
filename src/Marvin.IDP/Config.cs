// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Marvin.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                // map to user idenitiy claim
                new IdentityResources.OpenId(),
                // Profile scope: map to profile related claims such as given name and family name 
                new IdentityResources.Profile(),
                // Address scope
                new IdentityResources.Address(),
                // role is not standard scope. create it by ourselves
                new IdentityResource(
                    "roles", // scope name
                    "Your role(s)", // scope display name
                    new List<string>() { "role" }) // claims
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                // this will add "imagegalleryapi" to the aud claim of the issued access token.
                new ApiResource("imagegalleryapi", "Image Gallery API", 
                    // include the role claim into the returned access token.
                    new List<string>(){ "role"})
                {
                    Scopes =
                    {
                        "imagegalleryapi"
                    }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("imagegalleryapi", "Image Gallery API") // the backend ImageGallery.API server
            };

        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                // Our MVC web app in project ImageGallery.Client
                new Client()
                {
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient", // client application ID which client presents when request authroization code
                    AllowedGrantTypes = GrantTypes.Code, // Allow Authorization Code flow
                    RequirePkce = true,
                    RequireConsent = true, // display consent page after user login for scopes requested by client app
                    AllowRememberConsent = false, // for testing purpose, set to false so the consent is prompted at every login
                    RedirectUris = new List<string>() // Redirect user to Image Gallery MVC app after user authenticates to IdentityServer.
                    {
                        "https://localhost:44389/signin-oidc" // URL of the Image Gallery MVC application
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44389/signout-callback-oidc" // Redirect the user to the MVC application after signed out.
                    },
                    AllowedScopes =
                    {
                        // Specify scopes that client application can request access to.
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "imagegalleryapi"
                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256()) // client secret which client presents
                    }
                }
            };
    }
}
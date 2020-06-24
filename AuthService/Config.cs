// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace AuthService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("Photography.Post.API"),
                new ApiResource("Photography.User.API"),
                new ApiResource("Photography.Order.API"),
                new ApiResource("Photography.Notification.API"),
                new ApiResource("Arise.FileUploadService"),
                new ApiResource("Photography.ApiGateway")
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Photography.Post.API",
                        "Photography.User.API",
                        "Photography.Order.API",
                        "Photography.Notification.API",
                        "Photography.ApiGateway",
                        "Arise.FileUploadService"
                    }
                },
                // client credentials flow client
                new Client
                {
                    ClientId = "ro.client",
                    ClientName = "Resource Owner Client",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AccessTokenLifetime = 604800,
                    IdentityTokenLifetime = 604800,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Photography.Post.API",
                        "Photography.User.API",
                        "Photography.Order.API",
                        "Photography.Notification.API",
                        "Photography.ApiGateway",
                        "Arise.FileUploadService"
                    }
                },
                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "native.code",
                    ClientName = "native code",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    //RedirectUris = { "https://notused" },
                    //PostLogoutRedirectUris = { "https://notused" },
                    RedirectUris = { "http://127.0.0.1:45656" },
                    PostLogoutRedirectUris = { "http://127.0.0.1:45656" },

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Photography.Post.API",
                        "Photography.User.API",
                        "Photography.Order.API",
                        "Photography.Notification.API",
                        "Photography.ApiGateway",
                        "Arise.FileUploadService"
                    }
                },

                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "webclient",
                    ClientName = "Web Client",

                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://senseapi.ars-sense.com:5107/signin-oidc" },
                    //RedirectUris = { "http://121.196.28.120:5107/signin-oidc" },
                    //FrontChannelLogoutUri = "https://localhost:4001",
                    //PostLogoutRedirectUris = { "https://localhost:4001" },

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Photography.Post.API",
                        "Photography.User.API",
                        "Photography.Order.API",
                        "Photography.Notification.API",
                        "Photography.ApiGateway",
                        "Arise.FileUploadService"
                    }
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:5002/index.html",
                        "http://localhost:5002/callback.html",
                        "http://localhost:5002/silent.html",
                        "http://localhost:5002/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = 
                    { 
                        "openid", 
                        "profile", 
                        "Photography.Post.API", 
                        "Photography.User.API",
                        "Photography.Order.API",
                        "Photography.Notification.API",
                        "Photography.ApiGateway",
                        "Arise.FileUploadService" 
                    }
                }
            };
    }
}
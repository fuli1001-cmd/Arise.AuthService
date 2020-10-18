using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthService
{
    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            var lifetime = 604800;

            return new List<Client>
            {
                new Client
                {
                    ClientId = "FC6E2833-5DA0-AF61-F371-6D013BB384E6",
                    ClientName = "名冠充值平台管理",

                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("84B20313-29C0-B9F7-4EA2-0DFA28C15803".Sha256()) },

                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,

                    // The default ASP.NET OpenID Connect middleware uses yourapp.com/signin-oidc and yourapp.com/signout-callback-oidc endpoints to intercept and handle the login/logout hand-off from IdentityServer. 
                    // RedirectUri has to be set to signin-oidc if the default ASP.NET OpenID Connect middleware is using
                    RedirectUris = new List<string> { "http://121.196.28.120:20001/signin-oidc", "http://localhost:30001/signin-oidc" },
                    // RedirectUri has to be set to signout-callback-oidc if the default ASP.NET OpenID Connect middleware is using
                    PostLogoutRedirectUris = new List<string> { "http://121.196.28.120:20001/signout-callback-oidc", "http://localhost:30001/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        //IdentityServerConstants.StandardScopes.Email,
                        "role",
                        "Recharge.API"
                    }
                },
                new Client
                {
                    ClientId = "f76dfaaa-b34f-c600-da99-1d18f9625dbf",
                    ClientName = "江苏久雅",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("585a3c0a-a7c1-71b0-8ad8-b4b3120d04a9".Sha256()) },
                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,
                    AllowedScopes = new List<string> { "Recharge.API" }
                },
                new Client
                {
                    ClientId = "D24B382B-5A9F-6A48-9BB9-B980CAA664DE",
                    ClientName = "福禄",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("C1E3C946-48FA-7611-3EC2-70CCD31548BC".Sha256()) },
                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,
                    AllowedScopes = new List<string> { "Recharge.API" }
                },
                new Client
                {
                    ClientId = "F10745CD-C8DF-683E-C57D-AA4E749B0690",
                    ClientName = "福禄2号",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("9f561e4c-d00e-780d-5975-c68d2b125947".Sha256()) },
                    AccessTokenLifetime = lifetime,
                    IdentityTokenLifetime = lifetime,
                    AllowedScopes = new List<string> { "Recharge.API" }
                }
            };
        }
    }

    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { "Role" }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource
                {
                    Name = "Recharge.API",
                    DisplayName = "Recharge API",
                    Description = "Allow the application to access Recharge API on your behalf",
                    Scopes = new List<string> {"Recharge.API"},
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("Recharge.API")
            };
        }
    }

    internal class Users
    {
        public static List<TestUser> Get()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "scott",
                    Password = "password",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.Email, "scott@scottbrady91.com"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                }
            };
        }
    }
}

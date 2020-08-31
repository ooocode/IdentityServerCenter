// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServerCenter
{
    public class ClientInfo
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientPassword { get; set; }
        public List<string> ClientIps { get; set; }

        public string FrontChannelLogoutUri  { get; set; }
    }
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(){ DisplayName = "用户id" },
                new IdentityResources.Profile() {  DisplayName = "用户信息",
                    Description = "您的个人信息 (用户名、头像、角色等)", Required = true },
            };


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "API #1")
            };


        public static IEnumerable<Client> GetClients(IConfiguration Configuration)
        {
            var ips = Configuration.GetSection("Clients").Get<List<ClientInfo>>();
            foreach (var ip in ips)
            {
                var client = new Client
                {
                    ClientId = ip.ClientId,
                    ClientName = ip.ClientName,



                    AllowedGrantTypes = GrantTypes.Implicit,

                    //授权确认页面，false则跳过
                    RequireConsent = true,


                    RequireClientSecret = false,

                    
                    // RequirePkce = true,
                    ClientSecrets = { new Secret(ip.ClientPassword.Sha256()) },

                    //RedirectUris = { $"{ip.ClientIp}/signin-oidc" },
                    //FrontChannelLogoutUri = $"{ip.ClientIp}/signout-oidc",
                    //PostLogoutRedirectUris = { $"{ip.ClientIp}/signout-callback-oidc" },

                    RedirectUris = ip.ClientIps.Select(e => $"{e}/signin-oidc").ToList(),
                    PostLogoutRedirectUris = ip.ClientIps.Select(e => $"{e}/signout-callback-oidc").ToList(),

                    FrontChannelLogoutUri = ip.FrontChannelLogoutUri,

                    //BackChannelLogoutUri = "",
                    //FrontChannelLogoutUri = $"{ip.ClientIps}/signout-oidc",


                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" },


                    //允许返回Access Token
                    AllowAccessTokensViaBrowser = true,


                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,

                   
                };

                yield return client;
            }
        }

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

                    AllowedScopes = { "api1" }
                },

                new Client
                {
                    ClientId = "razor pages",
                    ClientName = "razor pages Client",

                    RequireClientSecret = false,

                    AllowedGrantTypes = GrantTypes.Implicit,
                   // RequirePkce = true,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:58174/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:58174/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:58174/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" },

                    
                //允许返回Access Token
                AllowAccessTokensViaBrowser = true,


                AlwaysSendClientClaims = true,
               // AlwaysIncludeUserClaimsInIdToken = true
                },


                // MVC client using code flow + pkce
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequirePkce = true,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:58174/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:58174/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:58174/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" }
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

                    AllowedScopes = { "openid", "profile", "api1" }
                }
            };
    }
}
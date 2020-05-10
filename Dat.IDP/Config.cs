// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Dat.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles","Your Role(s)",new List<string>(){"role" }),
                new IdentityResource("company","Company Name",new List<string>(){"company" })
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            { 
                new ApiResource("HomerApi","Homer Store Api v1", new List<string>(){"role","profile","company","given_name" })
            };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            {
                new Client
                {
                    AccessTokenLifetime=120,
                    AllowOfflineAccess=true,                    
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    SlidingRefreshTokenLifetime = 1296000, //15 days
                    UpdateAccessTokenClaimsOnRefresh=true,
                    ClientName = "Homer Store",
                    ClientId="homerClient",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,//GrantTypes.Code,
                    RequirePkce = true,
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:5001/signin-oidc"
                    },
                    PostLogoutRedirectUris =new List<string>()
                    {
                        "https://localhost:5001/signout-callback-oidc" //this is the UI implementing the ID4 server middleware
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "HomerApi",
                        "company"
                    },
                    ClientSecrets =
                    {
                        new Secret("eric.pham".Sha256())
                    }
                }
            };
        
    }
}
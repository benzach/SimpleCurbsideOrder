// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace Dat.IDP
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser
            {
                SubjectId="6766f7b0-7b36-4730-8915-3cce416ffea3",
                Username="Eric",
                Password="Password",
                Claims=new List<Claim>
                {
                    new Claim("given_name","Eric"),
                    new Claim("family_name","Doe"),
                    new Claim("address","1212 NW 12, okc, ok,73110"),
                    new Claim("role","Store Owner"),
                    new Claim("company","HEB")


                }
            },
            new TestUser
            {
                SubjectId="cbcf56c7-6c87-4ae3-8a8b-6a149fb0da14",
                Username="Jane",
                Password="Password",
                Claims=new List<Claim>
                {
                    new Claim("given_name","Jane"),
                    new Claim("family_name","Doe"),
                    new Claim("address","1313 nw 13, okc, ok, 73110"),
                    new Claim("role","Store Owner"),
                    new Claim("role","Admin"),
                    new Claim("company","Tiny Acquatic")
                }
            },
            new TestUser
            {
                SubjectId="f3c9ca7a-5e53-42a8-b42e-462855a7d56f",
                Username="Bob",
                Password="Password",
                Claims=new List<Claim>
                {
                    new Claim("given_name","Bob"),
                    new Claim("family_name","Doe"),
                    new Claim("address","1313 nw 13, okc, ok, 73110"),
                    new Claim("role","Admin"),
                    new Claim("company","Tiny Acquatic")
                }
            }
            //new TestUser{SubjectId = "818727", Username = "alice", Password = "alice", 
            //    Claims = 
            //    {
            //        new Claim(JwtClaimTypes.Name, "Alice Smith"),
            //        new Claim(JwtClaimTypes.GivenName, "Alice"),
            //        new Claim(JwtClaimTypes.FamilyName, "Smith"),
            //        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
            //        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            //        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
            //        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
            //    }
            //},
            //new TestUser{SubjectId = "88421113", Username = "bob", Password = "bob", 
            //    Claims = 
            //    {
            //        new Claim(JwtClaimTypes.Name, "Bob Smith"),
            //        new Claim(JwtClaimTypes.GivenName, "Bob"),
            //        new Claim(JwtClaimTypes.FamilyName, "Smith"),
            //        new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
            //        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
            //        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
            //        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
            //        new Claim("location", "somewhere")
            //    }
            //}
        };
    }
}
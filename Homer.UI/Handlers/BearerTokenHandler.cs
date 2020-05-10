using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Homer.UI.Handlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            _httpContextAccessor = httpContextAccessor?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //var token =await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var accessToken = await GetAccessTokenAsync();

            if(!string.IsNullOrEmpty(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
        public async Task<string> GetAccessTokenAsync()
        {
            var expireAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");
            //var expiredDate = DateTimeOffset.Parse(expireAt, CultureInfo.InvariantCulture);
            
            if (!string.IsNullOrEmpty(expireAt) && 
                DateTimeOffset.TryParse(expireAt,CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind,out var expiredDate) &&
                expiredDate.AddSeconds(-60).ToUniversalTime()>DateTime.UtcNow)
            {

                return await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            }
            var client = _httpClientFactory.CreateClient("IDPClient");

            var discoveryDoc =await client.GetDiscoveryDocumentAsync();
            var updatedTokens = new List<AuthenticationToken>();
            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            var refreshTokenResponse = await client.RequestRefreshTokenAsync(
                new RefreshTokenRequest
                {
                    Address=discoveryDoc.TokenEndpoint,
                    ClientId= "homerClient",
                    ClientSecret="eric.pham",
                    RefreshToken=refreshToken
                });
            
            updatedTokens.Add(new AuthenticationToken {
                Name = OpenIdConnectParameterNames.AccessToken,
                Value = refreshTokenResponse.AccessToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.IdToken,
                Value = refreshTokenResponse.IdentityToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = OpenIdConnectParameterNames.RefreshToken,
                Value = refreshTokenResponse.RefreshToken
            });
            updatedTokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshTokenResponse.ExpiresIn)).ToString("o", CultureInfo.InvariantCulture)
            });

            var currentAuthenticateResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            currentAuthenticateResult.Properties.StoreTokens(updatedTokens);
            //sign in
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, currentAuthenticateResult.Principal, currentAuthenticateResult.Properties);

            return refreshTokenResponse.AccessToken;


        }
    }
}

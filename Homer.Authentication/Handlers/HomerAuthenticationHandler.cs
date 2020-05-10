using Homer.Authentication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Homer.Authentication.Handlers
{
    public class HomerAuthenticationHandler : AuthenticationHandler<HomerAuthenticationSchemeOptions>
    {
        private readonly ITokenService _tokenService;
        public HomerAuthenticationHandler(IOptionsMonitor<HomerAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenService tokenService) : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            var authHeader = Request.Headers["Authorization"];
            if(string.IsNullOrEmpty(authHeader))
            {
                return AuthenticateResult.NoResult();
            }
            if(!authHeader.ToString().StartsWith("Bearer",StringComparison.InvariantCultureIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            string token = authHeader.ToString().Substring("bearer".Length).Trim();
            if(string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            var claims=_tokenService.GetClaims(token).ToList();
            var hasCompanyClaim = claims.Any(x => x.Type == "company");
            if(hasCompanyClaim)
            {
                claims.Add(new Claim("role", "Same Company"));
            }
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new GenericPrincipal(identity,new[] { "Same Company" });
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}

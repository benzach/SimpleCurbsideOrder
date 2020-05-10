using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Homer.Authentication.Services
{
    public class TokenService : ITokenService
    {
        private JwtSecurityTokenHandler _hander = new JwtSecurityTokenHandler();
        public TokenService()
        {
            
        }
        public IEnumerable<Claim> GetClaims(string token)
        {
            //var res = getClaims(token,_hander);
            var res=_hander.ReadJwtToken(token);
            return res.Claims;
        }
    }
}

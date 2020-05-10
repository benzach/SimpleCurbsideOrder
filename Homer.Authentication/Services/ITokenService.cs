using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Homer.Authentication.Services
{
    public interface ITokenService
    {
        IEnumerable<Claim> GetClaims(string token);
    }
}

using Homer.Authorization.Interfaces;
using Homer.Authorization.Requirements;
using Homer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Authorization.Services
{
    public class ResourceBasedAuthService : IResourceBasedAuthService
    {
        private readonly IAuthorizationService _authService;
        public ResourceBasedAuthService(IAuthorizationService authService)
        {
            _authService = authService;
        }

        public async Task<bool> AuthorizeAsync<T>(ClaimsPrincipal user, T resource) where T : ICompanyResource, IOwnerResource
        {
            var res = await _authService.AuthorizeAsync(user, resource,new SameCompanyRequirement());
            var resUser = await _authService.AuthorizeAsync(user, resource, new SameUserRequirement());
            if(res.Succeeded && resUser.Succeeded)
            {
                return true;
            }
            if(res.Failure.FailCalled || resUser.Failure.FailCalled)
            {
                return false;
            }
            return false;
        }
    }
}

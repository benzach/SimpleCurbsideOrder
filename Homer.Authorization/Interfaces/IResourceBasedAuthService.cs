using Homer.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Authorization.Interfaces
{
    public interface IResourceBasedAuthService
    {
        Task<bool> AuthorizeAsync<T>(ClaimsPrincipal user, T resource) where T:ICompanyResource,IOwnerResource;
    }
}

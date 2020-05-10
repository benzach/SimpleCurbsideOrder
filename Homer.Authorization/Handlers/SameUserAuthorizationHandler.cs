using Homer.Authorization.Models;
using Homer.Authorization.Requirements;
using Homer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Authorization.Handlers
{
    public class SameUserAuthorizationHandler : AuthorizationHandler<SameUserRequirement, IOwnerResource>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserRequirement requirement, IOwnerResource resource)
        {            
            var user = context.User.Claims.FirstOrDefault(x => x.Type == HomerClaimConstants.UserId);
            if(string.IsNullOrEmpty(resource.OwnerId) || !resource.OwnerId.Equals(user.Value, StringComparison.InvariantCultureIgnoreCase)){
                context.Fail();
            }else if(resource.OwnerId.Equals(user.Value, StringComparison.InvariantCultureIgnoreCase))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}

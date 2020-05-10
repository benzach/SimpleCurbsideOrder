using Homer.Authorization.Models;
using Homer.Authorization.Requirements;
using Homer.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homer.Authorization.Handlers
{
    public class SameCompanyAuthorizationHandler : AuthorizationHandler<SameCompanyRequirement, ICompanyResource>
    {
        public SameCompanyAuthorizationHandler()
        {
            Debug.WriteLine("test");
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameCompanyRequirement requirement, ICompanyResource resource)
        {
            var company = context.User.Claims.SingleOrDefault(x => x.Type == HomerClaimConstants.Company);
            if(company==null)
            {
                var test = context.User.Claims.SingleOrDefault(x => x.Type == "role");
                var roles = context.User.Claims.Where(x => x.Type == "role");
                if(context.User.Claims.Any(x=>x.Type=="role") && roles.Any(x=>x.Value==HomerRoleConstants.SameCompany))
                {
                    context.Succeed(requirement);
                }
            }else if(resource.CompanyName==default || !resource.CompanyName.Equals(company.Value,StringComparison.InvariantCultureIgnoreCase))
            {
                context.Fail();
            }else
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}

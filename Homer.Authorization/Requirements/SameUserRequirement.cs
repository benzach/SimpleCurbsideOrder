using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homer.Authorization.Requirements
{
    public class SameUserRequirement:IAuthorizationRequirement
    {
        public SameUserRequirement()
        {
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homer.Authorization.Models
{
    public static class AuthorizationResultExtension
    {
        public static bool Succeed(this AuthorizationResult authresult)
            => authresult.Succeeded || (authresult.Failure!=null && !authresult.Failure.FailCalled);
        public static bool Failed(this AuthorizationResult authresult)
            => authresult.Failure != null && authresult.Failure.FailCalled;
    }
}

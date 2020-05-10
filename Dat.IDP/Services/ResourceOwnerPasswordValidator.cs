using Dat.Data;
using IdentityModel;
using IdentityServer4.Validation;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dat.IDP.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserProfileRepository<IMongoDatabase> _userProfileRepository;
        public ResourceOwnerPasswordValidator(IUserProfileRepository<IMongoDatabase> userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if(await _userProfileRepository.ValidateCredentialsAsync(context.UserName, context.Password))
            {
                var user = await _userProfileRepository.FindByUsernameAsync(context.UserName);
                context.Result = new GrantValidationResult(user.SubjectId, OidcConstants.AuthenticationMethods.Password);
            }
        }
    }
}

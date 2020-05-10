using AutoMapper;
using Dat.Data;
using Dat.Domain;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dat.IDP.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly ILogger<CustomProfileService> _Logger;
        private readonly IUserProfileRepository<IMongoDatabase> _userRepository;
        private readonly IMapper _mapper;
        public CustomProfileService(ILogger<CustomProfileService> logger, IUserProfileRepository<IMongoDatabase> userRepository,IMapper mapper)
        {
            _Logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            _Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);
            var user = await _userRepository.FindBySubjectIdAsync(context.Subject.GetSubjectId());
            var userDto = _mapper.Map<UserProfileDTO>(user);
            context.IssuedClaims = userDto.Claims.ToList();
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userRepository.FindBySubjectIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}

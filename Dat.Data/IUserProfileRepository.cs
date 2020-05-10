using Dat.Database.Abstract;
using Dat.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dat.Data
{
    public interface IUserProfileRepository<TDatabase>:IRepository<TDatabase,UserProfile>
    {
        public Task<UserProfile> AutoProvisionUserAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
        //public Task<UserProfile> AutoProvisionUserAsync(string provider, string userId, List<Claim> claims,CancellationToken cancellationToken=default);
        //
        // Summary:
        //     Finds the user by external provider.
        //
        // Parameters:
        //   provider:
        //     The provider.
        //
        //   userId:
        //     The user identifier.
        public Task<UserProfile> FindByExternalProviderAsync(string provider, string userId, CancellationToken cancellationToken = default);
        //
        // Summary:
        //     Finds the user by subject identifier.
        //
        // Parameters:
        //   subjectId:
        //     The subject identifier.
        public Task<UserProfile> FindBySubjectIdAsync(string subjectId, CancellationToken cancellationToken = default);
        //
        // Summary:
        //     Finds the user by username.
        //
        // Parameters:
        //   username:
        //     The username.
        public Task<UserProfile> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);
        //
        // Summary:
        //     Validates the credentials.
        //
        // Parameters:
        //   username:
        //     The username.
        //
        //   password:
        //     The password.
        public Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
    }
}

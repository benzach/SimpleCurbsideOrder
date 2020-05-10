using Dat.Database.Abstract;
using Dat.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dat.Data
{
    public class UserProfileMongoRepository : IUserProfileRepository<IMongoDatabase>
    {
        private readonly IBackStoreContext<IMongoDatabase> _context;
        private readonly Func<string,HashedData> _HashedPerform;
        public UserProfileMongoRepository(IBackStoreContext<IMongoDatabase> context,Func<string,HashedData> hashedPerform)
        {
            _context = context;
            _HashedPerform = hashedPerform;
        }

        public Task<UserProfile> AutoProvisionUserAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
        {
            ////var datClaim = new DatClaim();
            ////datClaim.address = claims.FirstOrDefault(x => x.Type == nameof(datClaim.address))?.Value;
            ////datClaim.company= claims.FirstOrDefault(x => x.Type == nameof(datClaim.company))?.Value;
            ////datClaim.family_name=claims.FirstOrDefault(x => x.Type == nameof(datClaim.family_name))?.Value;
            ////datClaim.given_name= claims.FirstOrDefault(x => x.Type == nameof(datClaim.given_name))?.Value;
            ////datClaim.role= claims.FirstOrDefault(x => x.Type == nameof(datClaim.role))?.Value;
            //var userProfile = new UserProfile {
            //    SubjectId = Guid.NewGuid().ToString(),
            //    ProviderName = provider, 
            //    Username = userId, 
            //    Claims = claims };
            
            return _context.UpdateAsync(userProfile, cancellationToken);
        }

        public  Task<bool> DeleteAsync(string Id, CancellationToken cancellationToken = default)
        {
            return _context.DeleteAsync<UserProfile>(Id, cancellationToken); 
        }

        public async Task<UserProfile> FindByExternalProviderAsync(string provider, string userId, CancellationToken cancellationToken = default)
        {
            var res= await _context.GetByCustom(async db => {
                var providerFilter = Builders<UserProfile>.Filter.Eq(x => x.ProviderName, provider);
                var userFilter = Builders<UserProfile>.Filter.Eq(x => x.Username, userId);
                var andFilter = Builders<UserProfile>.Filter.And(new[] { providerFilter, userFilter });
                using (var cursor = await db.GetCollection<UserProfile>(Name()).FindAsync(andFilter, null, cancellationToken))
                {
                    if(cursor.MoveNext())
                    {
                        return cursor.Current;
                    }
                    return Enumerable.Empty<UserProfile>();
                }
            },cancellationToken);
            return res.SingleOrDefault();
        }

        public async Task<UserProfile> FindBySubjectIdAsync(string subjectId, CancellationToken cancellationToken = default)
        {
            var res = await _context.GetByCustom(async db => {
                var subjectFilter = Builders<UserProfile>.Filter.Eq(x => x.SubjectId, subjectId);
                using(var cur=await db.GetCollection<UserProfile>(Name()).FindAsync(subjectFilter,null,cancellationToken))
                {
                    if(cur.MoveNext())
                    {
                        return cur.Current;
                    }
                    return Enumerable.Empty<UserProfile>();
                }
            },cancellationToken);
            return res.SingleOrDefault();
        }

        public async Task<UserProfile> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            var res = await _context.GetByCustom(async db => {
                var userNameFilter = Builders<UserProfile>.Filter.Eq(x => x.Username, username);
                using (var cur = await db.GetCollection<UserProfile>(Name()).FindAsync(userNameFilter, null, cancellationToken))
                {
                    if (cur.MoveNext())
                    {
                        return cur.Current;
                    }
                    return Enumerable.Empty<UserProfile>();
                }
            }, cancellationToken);
            return res.SingleOrDefault();
        }
        public Task<IEnumerable<UserProfile>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _context.GetAll<UserProfile>(cancellationToken);
        }
        public Task<UserProfile> GetByIdAsync(string Id, CancellationToken cancellationToken = default)
        {
            return _context.GetByIdAsync<UserProfile>(Id, cancellationToken);
        }
        public string Name()
        {
            return nameof(UserProfile) + "s";
        }
        public Task<UserProfile> UpsertAsync(UserProfile data, CancellationToken cancellationToken = default)
        {
            if(string.IsNullOrEmpty(data.SubjectId))
            {
                data.SubjectId = Guid.NewGuid().ToString();
            }
            if (!string.IsNullOrEmpty(data.Password))
            {
                data.Password = _HashedPerform(data.Password).Value;
            }
            if (data.Claims!=null && data.Claims.roles != null)
            {
                data.Claims.roles = data.Claims.roles.Where(x => !string.IsNullOrEmpty(x) && !x.Equals("false", StringComparison.InvariantCultureIgnoreCase)).ToList();
            }
            return _context.UpsertAsync(data, cancellationToken);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            var res = await _context.GetByCustom(async db => {
                var userNameFilter = Builders<UserProfile>.Filter.Eq(x => x.Username, username);
                var passwordFilter = Builders<UserProfile>.Filter.Eq(x => x.Password, password);
                var andFilter = Builders<UserProfile>.Filter.And(new[] { userNameFilter, passwordFilter });
                using (var cursor = await db.GetCollection<UserProfile>(Name()).FindAsync(andFilter, null, cancellationToken))
                {
                    if (cursor.MoveNext())
                    {
                        return cursor.Current;
                    }
                    return Enumerable.Empty<UserProfile>();
                }
            }, cancellationToken);
            return res.Any();
        }
    }
}

using Homer.Models.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Homer.Models.Domain;
using MongoDB.Driver;
using System.Linq;
using Dat.Database.Abstract;

namespace Homer.Repositories
{
    public class StoresMongoRepository: IStoreRepository<IMongoDatabase>
    {
        private readonly IBackStoreContext<IMongoDatabase> _context;
        public StoresMongoRepository(IBackStoreContext<IMongoDatabase> context)
        {
            _context = context;
        }

        public Task<bool> DeleteAsync(string Id, CancellationToken cancellationToken=default)
        {
            return _context.DeleteAsync<Store>(Id, cancellationToken);
        }

        public Task<IEnumerable<Store>> GetAllAsync(CancellationToken cancellationToken=default)
        {
            return _context.GetAll<Store>(cancellationToken);
        }

        public async Task<IEnumerable<Store>> GetByAddressAsync(AddressInfo address, CancellationToken cancellation = default)
        {
            var Addressfilter = Builders<Store>.Filter.Regex(x => x.Address.Address,new MongoDB.Bson.BsonRegularExpression(address.Address,"i") );
            var StateFilter = Builders<Store>.Filter.Eq(x => x.Address.State, address.State);
            var CityFilter = Builders<Store>.Filter.Eq(x => x.Address.City, address.City);
            var ZipFilter = Builders<Store>.Filter.Eq(x => x.Address.Zip, address.Zip);
            List<FilterDefinition<Store>> filters = new List<FilterDefinition<Store>>();

            if(!string.IsNullOrWhiteSpace(address.Zip))
            {
                return await _context.GetByCustom<Store>(async mongodb =>
                {
                    using (var cursor = await mongodb.GetCollection<Store>(Name()).FindAsync(ZipFilter, null, cancellation))
                    {
                        while (cursor.MoveNext(cancellation))
                        {
                            return cursor.Current;
                        }
                        return Enumerable.Empty<Store>();
                    }
                }, cancellation);
            }
            if(!string.IsNullOrEmpty(address.City))
            {
                filters.Add(CityFilter);
            }
            if(!string.IsNullOrEmpty(address.State))
            {
                filters.Add(StateFilter);
            }
            if(!string.IsNullOrEmpty(address.Address))
            {
                filters.Add(Addressfilter);
            }
            if(filters.Count>0)
            {
                var andFilter = Builders<Store>.Filter.And(filters.ToArray());
                return await _context.GetByCustom<Store>(async mongo =>
                {
                    using(var cursor= await mongo.GetCollection<Store>(Name()).FindAsync(andFilter,null,cancellation))
                    {
                        if(cursor.MoveNext(cancellation))
                        {
                            return cursor.Current;
                        }
                        return Enumerable.Empty<Store>();
                    }
                },cancellation);
            }
            return Enumerable.Empty<Store>();
        }

        public Task<Store> GetByIdAsync(string Id, CancellationToken cancellationToken)
        {
            return _context.GetByIdAsync<Store>(Id, cancellationToken);
        }

        public  Task<IEnumerable<Store>> GetByProtectedResource(ProtectedProperties protectedProperties, CancellationToken cancellationToken=default)
        {
            var res =_context.GetByCustom(
                async db => {
                var companyFilter = Builders<Store>.Filter.Eq(x => x.CompanyName, protectedProperties.CompanyName);
                    //var ownerFilter = Builders<Store>.Filter.Eq(x => x.OwnerId, protectedProperties.OwnerId);
                    //var andFilter = Builders<Store>.Filter.And(new[] { companyFilter, ownerFilter });
                    var andFilter = Builders<Store>.Filter.And(new[] { companyFilter });
                    using (var cursor = await db.GetCollection<Store>(Name()).FindAsync<Store>(andFilter, null, cancellationToken))
                {
                    if(cursor.MoveNext(cancellationToken))
                    {
                        return cursor.Current;
                    }
                        return Enumerable.Empty<Store>();

                }
            }, cancellationToken);
            return res;
        }

        public string Name() => nameof(Store) + "s";

        public Task<Store> UpsertAsync(Store data, CancellationToken cancellationToken)
        {
            return _context.UpsertAsync(data, cancellationToken);
            //if(string.IsNullOrEmpty(data.Id))
            //{
            //    return _context.InsertAsync(data, cancellationToken);
            //}else
            //{
            //    return _context.UpdateAsync(data, cancellationToken);
            //}
        }
    }
}

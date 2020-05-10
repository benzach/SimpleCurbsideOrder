using Dat.Database.Abstract;
using Homer.Models.Domain;
using Homer.Models.Interfaces;
using Homer.Models.Options;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Homer.DbContext
{
    public class StoreMongoDbContext : IBackStoreContext<IMongoDatabase>
    {
        
        private readonly IMongoDatabase _db;
        public StoreMongoDbContext(IMongoClient client, IOptions<StoreOptions> options)
        {
            _db = client.GetDatabase(options.Value.Database);
        }


        public async Task<T> InsertAsync<T>(T data, CancellationToken cancellationToken) where T : IEntity
        {
            if (string.IsNullOrEmpty(data.Id))
            {
                data.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
                data.CreateDate = DateTime.UtcNow;
            }
            var filter = Builders<Store>.Filter.Eq("Id", data.Id);

            await _db.GetCollection<T>(Name<T>()).InsertOneAsync(data, new InsertOneOptions { BypassDocumentValidation = false }, cancellationToken);

            return data;
        }

        public async Task<T> UpdateAsync<T>(T data, CancellationToken cancellationToken) where T : IEntity
        {
            var id = data.Id;
            var filter = Builders<T>.Filter.Eq(x => x.Id, id);
            data.Id = null;
            //var jsonResolver = new IgnorableSerializerContractResolver();
            var serializerSetting = new JsonSerializerSettings() { 
                NullValueHandling=NullValueHandling.Ignore,
                DefaultValueHandling=DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(data, serializerSetting)) } };
            //var res = await _db.GetCollection<T>(Name<T>()).ReplaceOneAsync(filter, data, new ReplaceOptions { IsUpsert = false }, cancellationToken);
            var res = await _db.GetCollection<T>(Name<T>()).UpdateOneAsync(filter, bson,null,cancellationToken);
            data.Id = id;
            return data;

        }

        public async Task<T> GetByIdAsync<T>(string Id, CancellationToken cancellationToken) where T : IEntity
        {
            var filter = Builders<T>.Filter.Eq(entity => entity.Id, Id);
            using (var cursor = await _db.GetCollection<T>(Name<T>()).FindAsync(filter, null, cancellationToken))
            {
                while (await cursor.MoveNextAsync())
                {
                    var docs = cursor.Current;
                    foreach (var doc in docs)
                    {
                        return doc;
                    }
                }
            }
            return default;
        }
        public async Task<IEnumerable<T>> GetAll<T>(CancellationToken cancellationToken) where T : IEntity
        {
           
            using (var cursor = await _db.GetCollection<T>(Name<T>()).FindAsync(x => true))
            {
                while (cursor.MoveNext(cancellationToken))
                {
                    return cursor.Current;
                }
            }
            return Enumerable.Empty<T>();
        }

        public async Task<bool> DeleteAsync<T>(string Id, CancellationToken cancellationToken) where T: IEntity
        {
            var filter = Builders<T>.Filter.Eq(x => x.Id, Id);
            var res = await _db.GetCollection<T>(Name<T>()).DeleteOneAsync(filter, cancellationToken);
            return res.IsAcknowledged;
            
        }

        public string Name<T>() => typeof(T).Name + "s";

        public Task<IEnumerable<T>> GetByCustom<T>(Func<IMongoDatabase, Task<IEnumerable<T>>> filter, CancellationToken cancellation) where T : IEntity
        {
            return filter(_db);
        }

        public Task<T> UpsertAsync<T>(T data, CancellationToken cancellationToken) where T : IEntity
        {
            if(string.IsNullOrEmpty(data.Id))
            {
                return InsertAsync(data,cancellationToken);
            }else
            {
                return UpdateAsync<T>(data,cancellationToken);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dat.Database.Abstract
{
    public interface IBackStoreContext<TDatabase>
    {
        string Name<T>(); 
        Task<T> InsertAsync<T>(T data, CancellationToken cancellationToken) where T: IEntity;
        Task<T> UpdateAsync<T>(T data, CancellationToken cancellationToken) where T : IEntity;
        Task<T> UpsertAsync<T>(T data, CancellationToken cancellationToken) where T : IEntity;
        Task<T> GetByIdAsync<T>(string Id, CancellationToken cancellationToken) where T : IEntity;
        Task<bool> DeleteAsync<T>(string Id, CancellationToken cancellationToken) where T : IEntity;
        Task<IEnumerable<T>> GetAll<T>(CancellationToken cancellationToken) where T : IEntity;
        Task<IEnumerable<T>> GetByCustom<T>(Func<TDatabase, Task<IEnumerable<T>>> filter, CancellationToken cancellation) where T : IEntity;
    }
}
    
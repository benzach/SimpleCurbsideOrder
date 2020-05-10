using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dat.Database.Abstract
{
    public interface IRepository<TDatabase,T>
    {
        string Name();
        Task<T> GetByIdAsync(string Id, CancellationToken cancellationToken =default);  
        Task<T> UpsertAsync(T data, CancellationToken cancellationToken=default);
        Task<bool> DeleteAsync(string Id, CancellationToken cancellationToken=default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken=default);
    }
}

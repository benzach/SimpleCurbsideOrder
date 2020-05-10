using Dat.Database.Abstract;
using Homer.Models.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Homer.Models.Interfaces
{
    public interface IStoreRepository<TDatabase>:IRepository<TDatabase,Store>
    {
        Task<IEnumerable<Store>> GetByAddressAsync(AddressInfo address, CancellationToken cancellation = default);
        Task<IEnumerable<Store>> GetByProtectedResource(ProtectedProperties protectedProperties, CancellationToken cancellationToken=default);
    }
}

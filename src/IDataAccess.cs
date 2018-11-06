using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlDistributedCache
{
    public interface IDataAccess
    {
        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> func,bool isNoTracking = true) where TEntity : class;
        Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> func, CancellationToken cancellationToken = default(CancellationToken), bool isNoTracking = true) where TEntity : class;
        void Add<TEntity>(TEntity entity) where TEntity : class;
        Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(Expression<Func<TEntity, bool>> func) where TEntity : class;
        Task RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        Task RemoveAsync<TEntity>(Expression<Func<TEntity, bool>> func, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
        IDbContextTransaction GetTransaction();
        Task<IDbContextTransaction> GetTransactionAsync();
    }
}

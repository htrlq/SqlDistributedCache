using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlDistributedCache
{
    public class DataAccess<TContext> : IDataAccess where TContext : DbContext
    {
        private TContext Context { get; }

        public DataAccess(TContext context)
        {
            Context = context;
        }

        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> func, bool isNoTracking = true) where TEntity : class
        {
            if (isNoTracking)
            {
                return Context.Set<TEntity>()
                    .AsNoTracking()
                    .FirstOrDefault(func);
            }
            else
            {
                return Context.Set<TEntity>()
                    .FirstOrDefault(func);
            }
        }

        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> func, CancellationToken cancellationToken = default(CancellationToken), bool isNoTracking = true) where TEntity : class
        {
            if (isNoTracking)
            {
                return await Context.Set<TEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(func, cancellationToken);
            }
            else
            {
                return await Context.Set<TEntity>()
                    .FirstOrDefaultAsync(func, cancellationToken);
            }
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = Context.Set<TEntity>();

            dbSet.Remove(entity);
            Context.SaveChanges();
        }

        public void Remove<TEntity>(Expression<Func<TEntity, bool>> func) where TEntity : class
        {
            var dbSet = Context.Set<TEntity>();
            var list = dbSet
                    .Where(func)
                    .ToList();

            dbSet.RemoveRange(list);
            Context.SaveChanges();
        }

        public async Task RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            var dbSet = Context.Set<TEntity>();

            dbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveAsync<TEntity>(Expression<Func<TEntity, bool>> func, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            var dbSet = Context.Set<TEntity>();
            var list = await dbSet
                    .Where(func)
                    .ToListAsync();

            dbSet.RemoveRange(list);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public void Update<TEntity>(TEntity entity) where TEntity:class
        {
            Context.Set<TEntity>().Update(entity);
            Context.SaveChanges();
        }

        public async Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            Context.Set<TEntity>().Update(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }

        public IDbContextTransaction GetTransaction()
        {
            return Context.Database.CurrentTransaction ?? Context.Database.BeginTransaction();
        }

        public async Task<IDbContextTransaction> GetTransactionAsync()
        {
            return Context.Database.CurrentTransaction ?? await Context.Database.BeginTransactionAsync();
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            var dbSet = Context.Set<TEntity>();
            dbSet.Add(entity);
            Context.SaveChanges();
        }

        public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class
        {
            var dbSet = Context.Set<TEntity>();
            dbSet.Add(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}

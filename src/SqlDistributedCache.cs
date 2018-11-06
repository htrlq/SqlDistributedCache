using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======
using Microsoft.EntityFrameworkCore.Storage;
>>>>>>> Database access Restructure
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SqlDistributedCache.Model;
using System;
<<<<<<< HEAD
using System.Linq;
=======
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
>>>>>>> Database access Restructure
using System.Threading;
using System.Threading.Tasks;

namespace SqlDistributedCache
{
<<<<<<< HEAD
    public class SqlDistributedCache : IDistributedCache
    {
        private DistributedCacheContext Context { get; }
        private IOptions<DistributedCacheEntryOptions> Options { get; }

        public SqlDistributedCache(DistributedCacheContext context, IOptions<DistributedCacheEntryOptions> options)
        {
            Context = context;
            Options = options;
        }

        private EntityItem FirstOrDefault(string key)
        {
            var entity = Context.EntityItem
                .AsNoTracking()
                .FirstOrDefault(item => item.Key == key && !item.IsLock);

            return entity;
        }

        private async Task<EntityItem> FirstOrDefaultAsync(string key, CancellationToken token)
        {
            var entity = await Context.EntityItem
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Key == key && !item.IsLock, token);

            return entity;
        }

        public byte[] Get(string key)
        {
            var entity = FirstOrDefault(key);

            return entity.Value;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var entity = await FirstOrDefaultAsync(key, token);

            return entity.Value;
=======
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

    public class SqlDistributedCache : IDistributedCache
    {
        private IDataAccess DataAccess { get; }
        private IOptions<DistributedCacheEntryOptions> Options { get; }

        public SqlDistributedCache(IDataAccess dataAccess, IOptions<DistributedCacheEntryOptions> options)
        {
            DataAccess = dataAccess;
            Options = options;
>>>>>>> Database access Restructure
        }

        private DateTimeOffset GetNowTime(DistributedCacheEntryOptions options, DateTimeOffset now)
        {
            if (options.AbsoluteExpiration != null || options.AbsoluteExpirationRelativeToNow != null)
                throw new Exception("Unable Refresh End Time");

            return now.Add(options.SlidingExpiration.Value);
        }

        public void Refresh(string key)
        {
<<<<<<< HEAD
            using (var transaction = Context.Database.CurrentTransaction ?? Context.Database.BeginTransaction())
            {
                try
                {
                    var entity = FirstOrDefault(key);
=======
            using (var transaction = DataAccess.GetTransaction())
            {
                try
                {
                    var entity = DataAccess.FirstOrDefault<EntityItem>(item => item.Key == key, false);
>>>>>>> Database access Restructure

                    var now = DateTimeOffset.Now;
                    var configure = Options.Value;

                    entity.CreatedTime = now;
                    entity.EndTime = GetNowTime(Options.Value, now);

<<<<<<< HEAD
                    Context.Update(entity);
                    Context.SaveChanges();
=======
                    DataAccess.Update(entity);
>>>>>>> Database access Restructure

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
<<<<<<< HEAD
            using (var transaction = Context.Database.CurrentTransaction ?? await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await FirstOrDefaultAsync(key, token);
=======
            using (var transaction = await DataAccess.GetTransactionAsync())
            {
                try
                {
                    var entity = await DataAccess.FirstOrDefaultAsync<EntityItem>(item => item.Key == key, token , false);
>>>>>>> Database access Restructure

                    var now = DateTimeOffset.Now;
                    var configure = Options.Value;

                    entity.CreatedTime = now;
                    entity.EndTime = GetNowTime(Options.Value, now);

<<<<<<< HEAD
                    Context.Update(entity);
                    await Context.SaveChangesAsync();
=======
                    await DataAccess.UpdateAsync(entity);
>>>>>>> Database access Restructure

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        public void Remove(string key)
        {
<<<<<<< HEAD
            using (var transaction = Context.Database.CurrentTransaction ?? Context.Database.BeginTransaction())
            {
                try
                {
                    var entity = FirstOrDefault(key);

                    Context.Remove(entity);
                    Context.SaveChanges();
=======
            using (var transaction = DataAccess.GetTransaction())
            {
                try
                {
                    var entity = DataAccess.FirstOrDefault<EntityItem>(item => item.Key == key, false);

                    DataAccess.Remove(entity);
>>>>>>> Database access Restructure

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
<<<<<<< HEAD
            using (var transaction = Context.Database.CurrentTransaction ?? await Context.Database.BeginTransactionAsync())
            {
                try
                {
                    var entity = await FirstOrDefaultAsync(key, token);

                    Context.Remove(entity);
                    await Context.SaveChangesAsync();
=======
            using (var transaction = await DataAccess.GetTransactionAsync())
            {
                try
                {
                    var entity = await DataAccess.FirstOrDefaultAsync<EntityItem>(item => item.Key == key, token, false);

                    await DataAccess.RemoveAsync(entity);
>>>>>>> Database access Restructure

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        private EntityItem Builder(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var now = DateTimeOffset.Now;
            DateTimeOffset endTime;

            if (options.AbsoluteExpirationRelativeToNow != null)
            {
                var absoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow.Value;
                var dayString = DateTimeOffset.Now.ToString("yyyy/MM/dd");
                endTime = new DateTimeOffset(Convert.ToDateTime(dayString), absoluteExpirationRelativeToNow);
            }
            else if (options.AbsoluteExpiration != null)
            {
                endTime = options.AbsoluteExpiration.Value;
            }
            else if (options.SlidingExpiration != null)
            {
                endTime = DateTimeOffset.Now.Add(options.SlidingExpiration.Value);
            }
            else
            {
                throw new Exception("options invalid");
            }

            var entity = new EntityItem()
            {
                Key = key,
                Value = value,
                CreatedTime = now,
                EndTime = endTime
            };

            return entity;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
<<<<<<< HEAD
            using (var transaction = Context.Database.CurrentTransaction ?? Context.Database.BeginTransaction())
=======
            using (var transaction = DataAccess.GetTransaction())
>>>>>>> Database access Restructure
            {
                try
                {
                    var entity = Builder(key, value, options);

<<<<<<< HEAD
                    Context.Add(entity);
                    Context.SaveChanges();
=======
                    DataAccess.Add(entity);
>>>>>>> Database access Restructure

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
<<<<<<< HEAD
            using (var transaction = Context.Database.CurrentTransaction ?? await Context.Database.BeginTransactionAsync())
=======
            using (var transaction = await DataAccess.GetTransactionAsync())
>>>>>>> Database access Restructure
            {
                try
                {
                    var entity = Builder(key, value, options);

<<<<<<< HEAD
                    Context.Add(entity);
                    await Context.SaveChangesAsync();
=======
                    await DataAccess.AddAsync(entity);
>>>>>>> Database access Restructure

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }
<<<<<<< HEAD
=======

        public byte[] Get(string key)
        {
            var entity = DataAccess.FirstOrDefault<EntityItem>(item => item.Key == key && !item.IsLock && !item.IsDelete);

            return entity?.Value;
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var entity = await DataAccess.FirstOrDefaultAsync<EntityItem>(item => item.Key == key && !item.IsLock && !item.IsDelete, token);

            return entity?.Value;
        }
>>>>>>> Database access Restructure
    }
}

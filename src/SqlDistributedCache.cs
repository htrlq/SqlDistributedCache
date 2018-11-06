using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SqlDistributedCache.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SqlDistributedCache
{

    public class SqlDistributedCache : IDistributedCache
    {
        private IDataAccess DataAccess { get; }
        private IOptions<DistributedCacheEntryOptions> Options { get; }

        public SqlDistributedCache(IDataAccess dataAccess, IOptions<DistributedCacheEntryOptions> options)
        {
            DataAccess = dataAccess;
            Options = options;
        }

        private DateTimeOffset GetNowTime(DistributedCacheEntryOptions options, DateTimeOffset now)
        {
            if (options.AbsoluteExpiration != null || options.AbsoluteExpirationRelativeToNow != null)
                throw new Exception("Unable Refresh End Time");

            return now.Add(options.SlidingExpiration.Value);
        }

        public void Refresh(string key)
        {
            using (var transaction = DataAccess.GetTransaction())
            {
                try
                {
                    var entity = DataAccess.FirstOrDefault<EntityItem>(item => item.Key == key, false);

                    var now = DateTimeOffset.Now;
                    var configure = Options.Value;

                    entity.CreatedTime = now;
                    entity.EndTime = GetNowTime(Options.Value, now);

                    DataAccess.Update(entity);

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
            using (var transaction = await DataAccess.GetTransactionAsync())
            {
                try
                {
                    var entity = await DataAccess.FirstOrDefaultAsync<EntityItem>(item => item.Key == key, token , false);

                    var now = DateTimeOffset.Now;
                    var configure = Options.Value;

                    entity.CreatedTime = now;
                    entity.EndTime = GetNowTime(Options.Value, now);

                    await DataAccess.UpdateAsync(entity);

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
            using (var transaction = DataAccess.GetTransaction())
            {
                try
                {
                    var entity = DataAccess.FirstOrDefault<EntityItem>(item => item.Key == key, false);

                    DataAccess.Remove(entity);

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
            using (var transaction = await DataAccess.GetTransactionAsync())
            {
                try
                {
                    var entity = await DataAccess.FirstOrDefaultAsync<EntityItem>(item => item.Key == key, token, false);

                    await DataAccess.RemoveAsync(entity);

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
            using (var transaction = DataAccess.GetTransaction())
            {
                try
                {
                    var entity = Builder(key, value, options);

                    DataAccess.Add(entity);

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
            using (var transaction = await DataAccess.GetTransactionAsync())
            {
                try
                {
                    var entity = Builder(key, value, options);

                    await DataAccess.AddAsync(entity);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

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
    }
}

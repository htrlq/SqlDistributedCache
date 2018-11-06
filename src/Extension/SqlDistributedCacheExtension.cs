using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SqlDistributedCache.Extension
{
    public static class SqlDistributedCacheExtension
    {
        public static IServiceCollection AddSqlCache(this IServiceCollection services, Action<DistributedCacheEntryOptions> setupAction)
        {
            services.AddScoped<IDistributedCache, SqlDistributedCache>();
            services.Configure(setupAction);

            return services;
        }
    }
}

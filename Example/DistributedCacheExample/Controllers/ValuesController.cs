using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace DistributedCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IDistributedCache DistributedCache { get; set; }
        private IOptions<DistributedCacheEntryOptions> Options { get; }

        public ValuesController(IDistributedCache distributedCache, IOptions<DistributedCacheEntryOptions> options)
        {
            DistributedCache = distributedCache;
            Options = options;
        }

        public string Get()
        {
            var value = "boy".ToBytes();

            DistributedCache.Get("hello");

            DistributedCache.Refresh("hello");

            DistributedCache.Set("hello", value, Options.Value);

            return "hello";
        }
    }

    public static class StringExtension
    {
        public static byte[] ToBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}

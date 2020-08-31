using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Study.Service
{
    public class CacheService
    {
        private readonly IDistributedCache cache;

        public CacheService(IDistributedCache cache)
        {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task SetAsync<T>(string key, T obj,TimeSpan? timeSpan = null)
        {
            if (obj != null)
            {
                var str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
                await cache.SetStringAsync(key, str, new DistributedCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = timeSpan ?? TimeSpan.FromMinutes(5)
                }).ConfigureAwait(false);
            }
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await cache.GetStringAsync(key).ConfigureAwait(false);
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }

            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
            return obj;
        }

        public async Task RemoveAsync(string key)
        {
            await cache.RemoveAsync(key).ConfigureAwait(false);
        }
    }
}

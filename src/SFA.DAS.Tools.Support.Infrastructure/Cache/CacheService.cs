using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SFA.DAS.Tools.Support.Infrastructure.Cache;
public interface ICacheService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem, int expirationInHours = 1);
}

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem, int expirationInHours = 1)
    {
        var data = await cache.GetStringAsync(key);
        if (data != null)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        var item = await createItem();
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
        };

        await cache.SetStringAsync(key, JsonConvert.SerializeObject(item), cacheOptions);
        return item;
    }
}

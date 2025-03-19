using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace SFA.DAS.Tools.Support.Infrastructure.Cache;
public interface ICacheService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem, int expirationInHours = 1);
    Task SetAsync<T>(string key, T value, int expirationInHours = 1);
    Task<T> RetrieveFromCache<T>(string key);
    Task RemoveAsync(string key);
}

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem, int expirationInHours = 1)
    {
        var data = await cache.GetStringAsync(key);
        if (data != null)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        var item = await createItem();     
        await SetAsync(key, item, expirationInHours);
        return item;
    }

    public async Task<T> RetrieveFromCache<T>(string key)
    {
        var json = await cache.GetStringAsync(key);

        return json == null
                  ? default
                  : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, int expirationInHours = 1)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(expirationInHours)
        };

        await cache.SetStringAsync(key, JsonSerializer.Serialize(value), cacheOptions);       
    }

    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
    }
}

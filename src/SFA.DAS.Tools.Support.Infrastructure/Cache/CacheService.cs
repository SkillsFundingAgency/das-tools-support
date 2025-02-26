﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SFA.DAS.Tools.Support.Infrastructure.Cache;
public interface ICacheService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem, int expirationInMinutes = 60);
    Task SetAsync<T>(string key, T value, int expirationInMinutes = 60);
    Task<T> RetrieveFromCache<T>(string key);
    Task RemoveAsync(string key);
}

public class CacheService(IDistributedCache cache) : ICacheService
{
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem, int expirationInMinutes = 60)
    {
        var data = await cache.GetStringAsync(key);
        if (data != null)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        var item = await createItem();     
        await SetAsync(key, item, expirationInMinutes);
        return item;
    }

    public async Task<T> RetrieveFromCache<T>(string key)
    {
        var json = await cache.GetStringAsync(key);

        return json == null
                  ? default
                  : JsonConvert.DeserializeObject<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, int expirationInMinutes = 60)
    {
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expirationInMinutes)
        };

        await cache.SetStringAsync(key, JsonConvert.SerializeObject(value), cacheOptions);       
    }

    public async Task RemoveAsync(string key)
    {
        await cache.RemoveAsync(key);
    }
}

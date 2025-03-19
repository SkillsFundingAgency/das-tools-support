using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SFA.DAS.Tools.Support.Infrastructure.SessionStorage;
public interface ISessionStorageService
{
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem);
    Task SetAsync<T>(string key, T value);
    T RetrieveFromCache<T>(string key);
}

public class SessionStorageService(IHttpContextAccessor httpContextAccessor) : ISessionStorageService
{
    private ISession Session => httpContextAccessor.HttpContext?.Session;

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> createItem)
    {
        if (Session == null)
        {
            return await createItem();
        }

        var data = Session.GetString(key);
        if (data != null)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        var item = await createItem();
        await SetAsync(key, item);
        return item;
    }

    public T RetrieveFromCache<T>(string key)
    {
        if (Session == null)
        {
            return default;
        }

        var json = Session.GetString(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        if (Session == null)
        {
            return;
        }

        Session.SetString(key, JsonSerializer.Serialize(value));
        await Task.CompletedTask;
    }
}

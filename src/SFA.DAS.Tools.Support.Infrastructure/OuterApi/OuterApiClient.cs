using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Tools.Support.Infrastructure.Configuration;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi;

public class OuterApiClient : IOuterApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ToolsSupportOuterApiConfiguration _config;

    private const string SubscriptionKeyRequestHeaderKey = "Ocp-Apim-Subscription-Key";
    private const string VersionRequestHeaderKey = "X-Version";

    public OuterApiClient(HttpClient httpClient,
    ToolsSupportOuterApiConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _httpClient.BaseAddress = new Uri(config.ApiBaseUrl);
        _httpClient.Timeout = new TimeSpan(0, 5, 0);

    }

    public async Task<TResponse> Get<TResponse>(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        AddRequestHeaders(request);

        using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
        {
            return default;
        }

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        response.EnsureSuccessStatusCode();

        return default;
    }

    public async Task<TResponse> Post<TRequest, TResponse>(string url, TRequest content)
    {      
        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonConvert.SerializeObject(content), System.Text.Encoding.UTF8, "application/json")
        };

        AddRequestHeaders(request);

        using var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

        if (response.StatusCode.Equals(HttpStatusCode.NotFound))
        {
            return default;
        }

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        response.EnsureSuccessStatusCode();

        return default;
    }


    private void AddRequestHeaders(HttpRequestMessage request)
    {
        request.Headers.Add(SubscriptionKeyRequestHeaderKey, _config.SubscriptionKey);
        request.Headers.Add(VersionRequestHeaderKey, "1");
    }
}

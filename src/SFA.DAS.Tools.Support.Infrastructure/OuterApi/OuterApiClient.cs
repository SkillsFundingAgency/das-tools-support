using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Tools.Support.Infrastructure.Configuration;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi;

public class OuterApiClient(HttpClient httpClient,
    ToolsSupportOuterApiConfiguration config) : IOuterApiClient
{
    private const string SubscriptionKeyRequestHeaderKey = "Ocp-Apim-Subscription-Key";
    private const string VersionRequestHeaderKey = "X-Version";

    public async Task<TResponse> Get<TResponse>(string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);

        AddRequestHeaders(request);

        using var response = await httpClient.SendAsync(request).ConfigureAwait(false);

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
        request.Headers.Add(SubscriptionKeyRequestHeaderKey, config.SubscriptionKey);
        request.Headers.Add(VersionRequestHeaderKey, "1");
    }
}

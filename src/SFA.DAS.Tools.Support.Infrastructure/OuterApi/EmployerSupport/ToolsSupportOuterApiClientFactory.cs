using System;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.Tools.Support.Infrastructure.Configuration;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

public interface IToolsSupportOuterApiClientFactory
{
    IToolsSupportOuterApiClient CreateClient();
}
public class ToolsSupportOuterApiClientFactory(ToolsSupportOuterApiConfiguration configuration,
    ILoggerFactory loggerFactory) : IToolsSupportOuterApiClientFactory
{
    public IToolsSupportOuterApiClient CreateClient()
    {
        var httpClient = new HttpClientBuilder()
            .WithDefaultHeaders()
            .WithApimAuthorisationHeader(configuration)
            .WithLogging(loggerFactory)
            .Build();

        httpClient.BaseAddress = !configuration.ApiBaseUrl.EndsWith('/')
            ? new Uri($"{configuration.ApiBaseUrl}/")
            : new Uri(configuration.ApiBaseUrl);

        return new ToolsSupportOuterApiClient(new OuterApiClient(httpClient, configuration));
    }
}

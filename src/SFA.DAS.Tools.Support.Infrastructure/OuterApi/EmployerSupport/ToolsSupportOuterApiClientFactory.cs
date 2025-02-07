using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.Tools.Support.Infrastructure.Configuration;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

public interface IToolsSupportOuterApiClientFactory
{
    IToolsSupportOuterApiClient CreateClient();
}
public class ToolsSupportOuterApiClientFactory : IToolsSupportOuterApiClientFactory
{
    private readonly ToolsSupportOuterApiConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public ToolsSupportOuterApiClientFactory(ToolsSupportOuterApiConfiguration configuration,
    ILoggerFactory loggerFactory,
    IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _loggerFactory = loggerFactory;
    }
    public IToolsSupportOuterApiClient CreateClient()
    {
        var httpClient = new HttpClientBuilder()
            .WithDefaultHeaders()
            .WithApimAuthorisationHeader(_configuration)
            .WithLogging(_loggerFactory)
            .Build();

        httpClient.BaseAddress = !_configuration.ApiBaseUrl.EndsWith('/')
            ? new Uri($"{_configuration.ApiBaseUrl}/")
            : new Uri(_configuration.ApiBaseUrl);

        return new ToolsSupportOuterApiClient(new OuterApiClient(httpClient, _configuration, _loggerFactory.CreateLogger<OuterApiClient>(), _httpContextAccessor));
    }
}

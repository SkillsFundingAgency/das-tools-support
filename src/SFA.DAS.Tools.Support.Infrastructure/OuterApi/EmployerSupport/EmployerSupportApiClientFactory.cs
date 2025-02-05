using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.Http;
using SFA.DAS.Tools.Support.Infrastructure.Configuration;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

public interface IEmployerSupportApiClientFactory
{
    IEmployerSupportApiClient CreateClient();
}
public class EmployerSupportApiClientFactory : IEmployerSupportApiClientFactory
{
    private readonly EmployerSupportApiClientConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public EmployerSupportApiClientFactory(EmployerSupportApiClientConfiguration configuration,
    ILoggerFactory loggerFactory,
    IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _loggerFactory = loggerFactory;
    }
    public IEmployerSupportApiClient CreateClient()
    {
        var httpClient = new HttpClientBuilder()
            .WithDefaultHeaders()
            .WithApimAuthorisationHeader(_configuration)
            .WithLogging(_loggerFactory)
            .Build();

        httpClient.BaseAddress = !_configuration.ApiBaseUrl.EndsWith('/')
            ? new Uri($"{_configuration.ApiBaseUrl}/")
            : new Uri(_configuration.ApiBaseUrl);

        return new EmployerSupportApiClient(new OuterApiClient(httpClient, _configuration, _loggerFactory.CreateLogger<OuterApiClient>(), _httpContextAccessor));
    }
}

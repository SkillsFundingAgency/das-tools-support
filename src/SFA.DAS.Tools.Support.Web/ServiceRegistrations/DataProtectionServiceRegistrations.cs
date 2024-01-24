using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Tools.Support.Web.Configuration;
using StackExchange.Redis;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class DataProtectionServiceRegistrations
{
    private const string ApplicationName = "das-tools-service";

    public static IServiceCollection AddDataProtection(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            return services;
        }
        
        var config = configuration.GetSection(nameof(DataProtectionSettings)).Get<DataProtectionSettings>();

        if (config == null)
        {
            return services;
        }
        
        var redis = ConnectionMultiplexer
            .Connect($"{config.RedisConnectionString},{config.DataProtectionKeysDatabase}");

        services.AddDataProtection()
            .SetApplicationName(ApplicationName)
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

        return services;
    }
}
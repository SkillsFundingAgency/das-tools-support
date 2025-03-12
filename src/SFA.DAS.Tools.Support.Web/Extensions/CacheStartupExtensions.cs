using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.Tools.Support.Web.Configuration;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class CacheStartupExtensions
{
    public static IServiceCollection AddCache(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
    {
        var redisConfiguration = configuration.GetSection("DataProtectionSettings")
       .Get<DataProtectionSettings>();

        if (redisConfiguration == null)
        {
            return services;
        }

        if (environment.IsDevelopment())
        {
            services.AddDistributedMemoryCache();
        }
        else
        {
            services.AddStackExchangeRedisCache(
                options => { options.Configuration = redisConfiguration.RedisConnectionString; });
        }

        return services;
    }
}

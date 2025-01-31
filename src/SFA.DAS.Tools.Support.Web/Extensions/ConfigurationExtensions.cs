using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Tools.Support.Web.Configuration;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class ConfigurationExtensions
{
    public static bool UseDfESignIn(this IConfiguration configuration) => configuration.GetValue<bool>("UseDfESignIn");

    public static IConfiguration BuildDasConfiguration(this IConfiguration configuration)
    {
        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables();

        if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
        {

#if DEBUG
            config
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile("appsettings.Development.json", true);
#endif
            config.AddAzureTableStorage(options =>
            {
                options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                options.EnvironmentName = configuration["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
        }

        return config.Build();
    }

    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<ToolsSupportConfig>(configuration);
        services.AddSingleton(cfg => cfg.GetService<IOptions<ToolsSupportConfig>>().Value);
        
        return services;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Configuration;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class ConfigurationServiceRegistrations
{
    public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();

        services.Configure<ToolsSupportConfig>(configuration);
        services.AddSingleton(cfg => cfg.GetService<IOptions<ToolsSupportConfig>>().Value);

        var employerUserCfg = configuration.GetSection("EmployerUserClientApiConfiguration");
        services.AddTransient<IEmployerUsersApiConfiguration, EmployerUsersApiConfiguration>(provider => new EmployerUsersApiConfiguration
        {
            ApiBaseUrl = employerUserCfg.GetValue<string>("ApiBaseUrl"),
            IdentifierUri = employerUserCfg.GetValue<string>("IdentifierUri")
        });

        var accountCfg = configuration.GetSection("AccountClientApiConfiguration");
        services.AddTransient<IAccountApiConfiguration, AccountApiConfiguration>(s => new AccountApiConfiguration
        {
            ApiBaseUrl = accountCfg.GetValue<string>("ApiBaseUrl"),
            IdentifierUri = accountCfg.GetValue<string>("IdentifierUri")
        });

        services.Configure<CommitmentsClientApiConfiguration>(configuration.GetSection("CommitmentsClientApiConfiguration"));
        var claimsConfig = new ClaimsConfiguration();

        services.AddSingleton<IOptions<ClaimsConfiguration>>(new OptionsWrapper<ClaimsConfiguration>(claimsConfig));

        services.Configure<ToolsSupportOuterApiConfiguration>(configuration.GetSection(nameof(ToolsSupportOuterApiConfiguration)));
        services.AddSingleton(cfg => cfg.GetService<IOptions<ToolsSupportOuterApiConfiguration>>().Value);

        var encodingConfigJson = configuration.GetSection("SFA.DAS.Encoding").Value;
        var encodingConfig = JsonConvert.DeserializeObject<EncodingConfig>(encodingConfigJson);
        services.AddSingleton(encodingConfig);

        return services;
    }
}
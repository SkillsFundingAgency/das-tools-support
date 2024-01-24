using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Mapping;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration _configuration, bool useDfeSignIn)
    {
        services.Configure<CommitmentsClientApiConfiguration>(_configuration.GetSection("CommitmentsClientApiConfiguration"));
        var claimsConfig = new ClaimsConfiguration(useDfeSignIn);
        services.AddSingleton<IOptions<ClaimsConfiguration>>(new OptionsWrapper<ClaimsConfiguration>(claimsConfig));
        services.AddTransient<IEmployerCommitmentsService, EmployerCommitmentsService>();            
        services.AddAutoMapper(config =>
        {
            config.AddProfile<AutoMapperProfile>();
        }, typeof(Startup));
            
        services.AddSingleton<ICommitmentsApiClientFactory>(
            x => new CommitmentsApiClientFactory(
                x.GetService<IOptions<CommitmentsClientApiConfiguration>>().Value, 
                x.GetService<ILoggerFactory>()));

        services.AddTransient(provider => provider.GetRequiredService<ICommitmentsApiClientFactory>().CreateClient());
            
        services.AddTransient<IEmployerAccountUsersService, EmployerAccountUsersService>();
            
        var accountCfg = _configuration.GetSection("AccountClientApiConfiguration");
        services.AddTransient<IAccountApiConfiguration, AccountApiConfiguration>(s => 
        {
            return new AccountApiConfiguration()
            {
                ApiBaseUrl = accountCfg.GetValue<string>("ApiBaseUrl"),
                IdentifierUri = accountCfg.GetValue<string>("IdentifierUri")
            };
        });

        services.AddScoped<IAccountApiClient, AccountApiClient>();

        services.AddTransient<IEmployerUsersService, EmployerUsersService>();

        var employerUserCfg = _configuration.GetSection("EmployerUserClientApiConfiguration");
        services.AddTransient<IEmployerUsersApiConfiguration, EmployerUsersApiConfiguration>(s => 
        {
            return new EmployerUsersApiConfiguration
            {
                ApiBaseUrl = employerUserCfg.GetValue<string>("ApiBaseUrl"),
                IdentifierUri = employerUserCfg.GetValue<string>("IdentifierUri")
            };
        });
        services.AddScoped<IEmployerUsersApiClient, EmployerUsersApiClient>();

        return services;
    }
}
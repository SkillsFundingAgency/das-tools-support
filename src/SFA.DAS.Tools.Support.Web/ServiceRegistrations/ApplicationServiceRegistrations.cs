﻿using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Mapping;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class ApplicationServiceRegistrations
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile<AutoMapperProfile>();
        }, typeof(Startup));

        services.AddSingleton<ICommitmentsApiClientFactory>(
            x => new CommitmentsApiClientFactory(
                x.GetService<IOptions<CommitmentsClientApiConfiguration>>().Value,
                x.GetService<ILoggerFactory>()));

        services.AddTransient(provider => provider.GetRequiredService<ICommitmentsApiClientFactory>().CreateClient());

        services.AddScoped<IAccountApiClient, AccountApiClient>();
        services.AddScoped<IEmployerUsersApiClient, EmployerUsersApiClient>();

        services.AddTransient<IEmployerUsersService, EmployerUsersService>();
        services.AddTransient<IEmployerAccountUsersService, EmployerAccountUsersService>();
        services.AddTransient<IEmployerCommitmentsService, EmployerCommitmentsService>();

        services.AddTransient<IAuthorizationProvider, AuthorizationProvider>();

        services.AddTransient<IToolsSupportOuterApiClientFactory, ToolsSupportOuterApiClientFactory>();
        services.AddSingleton<IToolsSupportOuterApiClient>(provider =>
        {
            var factory = provider.GetService<IToolsSupportOuterApiClientFactory>();
            return factory.CreateClient();
        });

        services.AddTransient<IEncodingService, EncodingService>();

        return services;
    }
}
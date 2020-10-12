using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public static class IoC
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration _configuration)
        {
            services.Configure<CommitmentsClientApiConfiguration>(_configuration.GetSection("CommitmentsClientApiConfiguration"));
            services.AddTransient<IEmployerCommitmentsService, EmployerCommitmentsService>();
            services.AddAutoMapper(config =>
            {
                config.ConfigureAutoMapper();
            }, typeof(Startup));
            services.AddSingleton<ICommitmentsApiClientFactory>(x =>
            {
                var config = x.GetService<IOptions<CommitmentsClientApiConfiguration>>().Value;
                var loggerFactory = x.GetService<ILoggerFactory>();
                return new CommitmentsApiClientFactory(config, loggerFactory);
            });
            services.AddTransient<ICommitmentsApiClient>(provider => provider.GetRequiredService<ICommitmentsApiClientFactory>().CreateClient());

            return services;
        }
    }
}

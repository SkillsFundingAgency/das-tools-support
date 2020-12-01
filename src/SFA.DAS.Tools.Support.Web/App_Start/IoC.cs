using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Web.Configuration;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public static class IoC
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration _configuration)
        {
            services.Configure<CommitmentsClientApiConfiguration>(_configuration.GetSection("CommitmentsClientApiConfiguration"));
            services.Configure<AzureAdConfiguration>(_configuration.GetSection("AzureAdConfiguration"));
            services.Configure<ClaimsConfiguration>(_configuration.GetSection("ClaimsConfiguration"));
            services.AddTransient<IEmployerCommitmentsService, EmployerCommitmentsService>();
            services.AddAutoMapper(config =>
            {
                config.AddProfile<AutoMapperProfile>();
            }, typeof(Startup));
            
            services.AddSingleton<ICommitmentsApiClientFactory>(
                x => new CommitmentsApiClientFactory(
                    x.GetService<IOptions<CommitmentsClientApiConfiguration>>().Value, 
                    x.GetService<ILoggerFactory>()));

            services.AddTransient<ICommitmentsApiClient>(provider => provider.GetRequiredService<ICommitmentsApiClientFactory>().CreateClient());

            return services;
        }
    }
}

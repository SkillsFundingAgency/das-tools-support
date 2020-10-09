using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Infrastructure.Helpers;
using AutoMapper;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.Models;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

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
                config.CreateMap<ApprenticeshipSummaryResult, StopApprenticeshipConfirmationViewModel>();
                config.CreateMap<GetApprenticeshipResult, StopApprenticeshipViewModel>();
                config.CreateMap<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDto>();
                config.CreateMap<GetApprenticeshipResponse, ApprenticeshipDto>();
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

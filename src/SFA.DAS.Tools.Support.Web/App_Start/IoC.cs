using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Core.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using System.Reflection;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public static class IoC
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration _configuration)
        {
            services.Configure<CommitmentsApiClientConfiguration>(_configuration.GetSection("CommitmentsApiClientConfiguration"));
            services.AddTransient<IEmployerCommitmentsService, EmployerCommitmentsService>();
            services.AddScoped<IEmployerCommitmentApi, EmployerCommitmentApi>(provider =>
            {
                var config = _configuration.GetSection("CommitmentsApiClientConfiguration").Get<CommitmentsApiClientConfiguration>();
                var httpClient = new HttpClientBuilder().WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config)).Build();
                return new EmployerCommitmentApi(httpClient, config);
            });

            return services;
        }
    }
}

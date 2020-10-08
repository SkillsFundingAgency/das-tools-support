using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Helpers
{
    public static class AutoMapperConfiguration
    {
        public static void ConfigureAutoMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<ApprenticeshipDetailsResponse, ApprenticeshipSummaryResult>();
        }

    }
}

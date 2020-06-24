using AutoMapper;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Helpers
{
    public static class AutoMapperConfiguration
    {
        public static void ConfigureAutoMapper(this IMapperConfigurationExpression config)
        {
            config.CreateMap<Apprenticeship, ApprenticeshipSummaryResult>();
        }

    }
}

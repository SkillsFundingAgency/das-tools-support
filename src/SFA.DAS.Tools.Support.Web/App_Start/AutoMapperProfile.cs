using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApprenticeshipDto, StopApprenticeshipViewModel>()
                .ForMember(dest => dest.ApprenticeshipId, m => m.MapFrom(u => u.Id));
            CreateMap<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDto>();
            CreateMap<GetApprenticeshipResponse, ApprenticeshipDto>()
                .ForMember(dest => dest.ApprenticeshipStatus, m => m.MapFrom(u => u.Status));
        }

    }
}

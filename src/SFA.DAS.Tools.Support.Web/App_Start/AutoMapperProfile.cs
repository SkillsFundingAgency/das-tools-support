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
            CreateMap<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDto>();
            CreateMap<GetApprenticeshipResponse, ApprenticeshipDto>()
                .ForMember(dest => dest.ApprenticeshipStatus, m => m.MapFrom(u => u.Status));
            CreateMap<ApprenticeshipDto, StopApprenticeshipRow>()
                .ForMember(dest => dest.Status, m => m.MapFrom(u => u.ApprenticeshipStatus.ToString()))
                .ForMember(dest => dest.AccountId, m => m.MapFrom(u => u.EmployerAccountId));
        }

    }
}
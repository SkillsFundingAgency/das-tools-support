using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse, ApprenticeshipDto>()
            .ForMember(dest => dest.Ukprn, m => m.MapFrom(u => u.ProviderId));
        
        CreateMap<GetApprenticeshipResponse, ApprenticeshipDto>()
            .ForMember(dest => dest.ApprenticeshipStatus, m => m.MapFrom(u => u.Status))
            .ForMember(dest => dest.Ukprn, m => m.MapFrom(u => u.ProviderId));
        
        CreateMap<TeamMemberViewModel, AccountUserDto>();
        
        CreateMap<ApprenticeshipDto, StopApprenticeshipRow>()
            .ForMember(dest => dest.Status, m => m.MapFrom(u => u.ApprenticeshipStatus.ToString()))
            .ForMember(dest => dest.AccountId, m => m.MapFrom(u => u.EmployerAccountId))
            .ForMember(dest => dest.StatusDate, m => m.MapFrom(u => MapStatusDate(u)));
        
        CreateMap<ApprenticeshipDto, PauseApprenticeshipRow>()
            .ForMember(dest => dest.Status, m => m.MapFrom(u => u.ApprenticeshipStatus.ToString()))
            .ForMember(dest => dest.AccountId, m => m.MapFrom(u => u.EmployerAccountId))
            .ForMember(dest => dest.StatusDate, m => m.MapFrom(u => MapStatusDate(u)));
        
        CreateMap<ApprenticeshipDto, ResumeApprenticeshipRow>()
            .ForMember(dest => dest.Status, m => m.MapFrom(u => u.ApprenticeshipStatus.ToString()))
            .ForMember(dest => dest.AccountId, m => m.MapFrom(u => u.EmployerAccountId))
            .ForMember(dest => dest.StatusDate, m => m.MapFrom(u => MapStatusDate(u)));
        
        CreateMap<AccountUserDto, AccountUserRow>();
    }

    private static DateTime? MapStatusDate(ApprenticeshipDto apprenticeshipDto)
    {
        switch(apprenticeshipDto.ApprenticeshipStatus)
        {
            case ApprenticeshipStatus.Stopped:
                return apprenticeshipDto.StopDate;
            case ApprenticeshipStatus.Paused:
                return apprenticeshipDto.PauseDate;                
            default:
                return null;
        }
    }
}
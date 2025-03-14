using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Mapping;

public class ApprenticeshipUlnSummaryMapper(IEncodingService encodingService) : IMapper<ApprovedApprenticeshipUlnSummary, ApprenticeshipUlnSummary>
{
    public ApprenticeshipUlnSummary Map(ApprovedApprenticeshipUlnSummary a)
    {
        return new ApprenticeshipUlnSummary
            {
            Id = a.Id,
            HashedId = encodingService.Encode(a.Id, EncodingType.ApprenticeshipId),
            HashedAccountId = encodingService.Encode(a.EmployerAccountId, EncodingType.AccountId),
            ProviderId = a.ProviderId,
            EmployerName = a.EmployerName,
            DisplayName = $"{a.FirstName} {a.LastName}",
            TrainingDates = $"{a.StartDate.ToString("MM/yy")} to {a.EndDate.ToString("MM/yy")}",
            StatusDescription = a.Status
        };
    }
}
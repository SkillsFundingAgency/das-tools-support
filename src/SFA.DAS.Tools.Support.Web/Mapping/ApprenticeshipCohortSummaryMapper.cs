using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Mapping;

public interface IMapper<in T1, out T2>
    where T1 : class
    where T2 : class
{
    T2 Map(T1 source);
}

public class ApprenticeshipCohortSummaryMapper(IEncodingService encodingService) : IMapper<ApprovedApprenticeshipCohortSummary, ApprenticeshipCohortSummary>
{
    public ApprenticeshipCohortSummary Map(ApprovedApprenticeshipCohortSummary a)
    {
        return new ApprenticeshipCohortSummary
        {
            Id = a.Id,
            HashedId = encodingService.Encode(a.Id, EncodingType.ApprenticeshipId),
            Uln = a.Uln,
            DisplayName = $"{a.FirstName} {a.LastName}",
            DateOfBirth = a.DateOfBirth,
            TrainingDates = $"{a.StartDate.ToString("MM/yy")} to {a.EndDate.ToString("MM/yy")}",
            StatusDescription = a.Status
        };
    }
}


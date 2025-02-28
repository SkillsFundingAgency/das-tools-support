using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Encoding;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class CohortDetailsViewModel
{
    public long CohortId { get; set; }
    public string HashedAccountId { get; set; }
    public string EmployerAccountName { get; set; }
    public string ProviderName { get; set; }
    public long UkPrn { get; set; }
    public string CohortReference { get; set; }
    public string CohortStatus { get; set; }
    public List<ApprenticeshipCohortSummary> Apprenticeships { get; set; } = null;
}

public class ApprenticeshipCohortSummary
{
    public long Id { get; set; }
    public string HashedId { get; set; }
    public string Uln { get; set; }
    public string DisplayName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string TrainingDates { get; set; }
    public string StatusDescription { get; set; }

    public static ApprenticeshipCohortSummary MapFrom(ApprovedApprenticeshipCohortSummary a, IEncodingService encodingService)
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
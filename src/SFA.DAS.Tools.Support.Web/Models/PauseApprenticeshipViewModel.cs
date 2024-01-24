namespace SFA.DAS.Tools.Support.Web.Models;

public class PauseApprenticeshipViewModel : ApprenticeshipViewModel
{
    public IEnumerable<PauseApprenticeshipRow> Apprenticeships { get; set; }
    public bool ApprenticesPausedCompleted => Apprenticeships != null && Apprenticeships.All(a => a.ApiSubmissionStatus == SubmissionStatus.Successful);
    public string GetApprenticesTableData() => JsonSerializer.Serialize(Apprenticeships, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
namespace SFA.DAS.Tools.Support.Web.Models;

public class ResumeApprenticeshipViewModel : ApprenticeshipViewModel
{
    public IEnumerable<ResumeApprenticeshipRow> Apprenticeships { get; set; }
    public bool ApprenticesResumedCompleted => Apprenticeships != null && Apprenticeships.All(a => a.ApiSubmissionStatus == SubmissionStatus.Successful);
    public string GetApprenticesTableData() => JsonSerializer.Serialize(Apprenticeships, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
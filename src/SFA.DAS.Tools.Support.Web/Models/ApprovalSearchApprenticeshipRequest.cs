namespace SFA.DAS.Tools.Support.Web.Models;

public record ApprovalSearchApprenticeshipRequest
{
    public string EmployerName { get; init; }
    public string CourseName { get; init; }
    public string ProviderName { get; init; }
    public string ApprenticeNameOrUln { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string SelectedStatus { get; init; }
    public long? Ukprn { get; init; }
    public string Act { get; init; }

    public ApprovalSearchApprenticeshipRequest(string employerName, string courseName, string providerName, string apprenticeNameOrUln, DateTime? startDate, DateTime? endDate, string selectedStatus, long? ukprn, string act)
    {
        EmployerName = employerName;
        CourseName = courseName;
        ProviderName = providerName;
        ApprenticeNameOrUln = apprenticeNameOrUln;
        StartDate = startDate;
        EndDate = endDate;
        SelectedStatus = selectedStatus;
        Ukprn = ukprn;
        Act = act;
    }
}
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
}
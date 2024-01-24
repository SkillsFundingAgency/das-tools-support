namespace SFA.DAS.Tools.Support.Web.Models;

public class ApprenticeshipRow
{ 
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Uln { get; set; }
    public string EmployerName { get; set; }
    public string ProviderName { get; set; }
    public long Ukprn { get; set; }
    public string CourseName { get; set; }
    public string Status { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string EnteredDate { get; set; }
    public SubmissionStatus ApiSubmissionStatus { get; set; }
    public string ApiErrorMessage { get; set; }
    public long AccountLegalEntityId { get; set; }
    public string CohortReference { get; set; }
    public DateTime? StatusDate { get; set; }
}
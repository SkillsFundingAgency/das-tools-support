namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class CommitmentSearchViewModel : AccountDetailsBaseViewModel
{
    public string SearchTerm { get; set; }
    public ApprenticeshipSearchType SearchType { get; set; } 
}

public enum ApprenticeshipSearchType
{
    SearchByUln,
    SearchByCohort
}

public enum MatchFailure
{
    NoneFound,
    AccessDenied
}

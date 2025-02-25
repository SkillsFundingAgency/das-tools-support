namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class CommitmentSearchModel
{
    public string SearchTerm { get; set; }
    public ApprenticeshipSearchType SearchType { get; set; } = ApprenticeshipSearchType.SearchByUln; 
    public List<MatchedApprenticeship> Apprenticeships { get; set; } = null;
}

public enum ApprenticeshipSearchType
{
    SearchByUln,
    SearchByCohort
}

public class MatchedApprenticeship
{
    public string HashedId { get; set; }
    public long Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}
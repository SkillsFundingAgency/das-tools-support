namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class ChallengeViewModel : AccountDetailsBaseViewModel
{
    public string Id { get; set; }

    public List<int> Characters { get; set; }

    public bool HasError { get; set; }
}

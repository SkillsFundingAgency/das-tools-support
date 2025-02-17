namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class EmployerUserSearchModel
{
    public string Email { get; set; }
    public List<MatchedUser> Users { get; set; } = null;
}

public class MatchedUser
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string DisplayName { get; set; }
}
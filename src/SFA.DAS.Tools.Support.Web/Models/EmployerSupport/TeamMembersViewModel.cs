using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class TeamMembersViewModel : AccountDetailsBaseViewModel
{
    public ICollection<TeamMember> TeamMembers { get; set; }
}

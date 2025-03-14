using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class TeamMemberActionConfirmationViewModel : AccountDetailsBaseViewModel
{
    public bool Success { get; set; }
    public string MemberEmail { get; set; }
    public string HashedAccountId { get; set; }
    public Role Role { get; set; }
    public TeamMemberAction TeamMemberAction { get; set; }
}

public enum TeamMemberAction
{
    InviteNewTeamMember,
    ResendInvitation,
    ChangeUserRole
}
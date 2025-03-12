using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class AccountDetailsViewModelExtensions
{
    public static string GetTeamMemberStatus(this InvitationStatus status)
    {
        switch (status)
        {
            case InvitationStatus.Accepted:
                return "Active";
            case InvitationStatus.Pending:
                return "Invitation awaiting response";
            case InvitationStatus.Expired:
                return "Invitation expired";
            case InvitationStatus.Deleted:
                return string.Empty;
            default:
                return string.Empty;
        }
    }
}

using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class InvitationViewModel : AccountDetailsBaseViewModel
{
    public string Email { get; set; }
    public string HashedAccountId { get; set; }
    public string FullName { get; set; }
    public Role Role { get; set; } = Role.None;
}

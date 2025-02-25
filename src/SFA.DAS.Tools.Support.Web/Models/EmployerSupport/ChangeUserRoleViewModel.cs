using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

public class ChangeUserRoleViewModel : AccountDetailsBaseViewModel
{
    public string FullName { get; set; }
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}

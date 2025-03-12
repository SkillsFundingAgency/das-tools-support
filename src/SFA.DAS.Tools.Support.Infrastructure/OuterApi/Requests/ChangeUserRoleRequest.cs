using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;

public class ChangeUserRoleRequest
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}

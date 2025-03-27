using MediatR;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;

public class ChangeUserRoleCommand : IRequest<ChangeUserRoleCommandResult>
{
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; }
}
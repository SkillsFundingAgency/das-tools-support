using MediatR;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;

public class SendTeamMemberInviteCommand : IRequest<SendTeamMemberInviteCommandResult>
{
    public string HashedAccountId { get; set; }
    public string FullName { get; set; }
    public string Email{ get; set; }
    public Role Role { get; set; }
}

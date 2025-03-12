using MediatR;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;

public class ResendTeamMemberInvitationCommand : IRequest<ResendTeamMemberInvitationCommandResult>
{
    public string HashedAccountId { get; set; } = "";
    public string Email { get; set; } = "";
}

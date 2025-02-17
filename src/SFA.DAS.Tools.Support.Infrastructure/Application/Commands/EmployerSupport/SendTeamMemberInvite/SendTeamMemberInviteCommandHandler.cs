using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;

public class SendTeamMemberInviteCommandHandler(
    IToolsSupportApimService employerSupportApiClient,
    ILogger<SendTeamMemberInviteCommandHandler> logger)
    : IRequestHandler<SendTeamMemberInviteCommand, SendTeamMemberInviteCommandResult>
{
    public async Task<SendTeamMemberInviteCommandResult> Handle(SendTeamMemberInviteCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var data = new SendInvitationRequest
            {
                HashedAccountId = command.HashedAccountId,
                FullName = command.FullName,
                Email = command.Email,
                Role = command.Role
            };

            await employerSupportApiClient.SendInvitation(data, cancellationToken);

            return new SendTeamMemberInviteCommandResult { Success = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in SendTeamMemberInviteCommandHandler");
            return new SendTeamMemberInviteCommandResult { Success = false };
        }
    }
}

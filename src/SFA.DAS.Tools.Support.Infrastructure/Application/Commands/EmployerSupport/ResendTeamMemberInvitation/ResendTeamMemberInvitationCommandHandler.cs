using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;

public class ResendTeamMemberInvitationCommandHandler(IToolsSupportApimService employerSupportApiClient,
    ILogger<ResendTeamMemberInvitationCommandHandler> logger)
    : IRequestHandler<ResendTeamMemberInvitationCommand, ResendTeamMemberInvitationCommandResult>
{
    public async Task<ResendTeamMemberInvitationCommandResult> Handle(ResendTeamMemberInvitationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var data = new ResendInvitationRequest
            {
                HashedAccountId = command.HashedAccountId,
                Email = command.Email
            };

            await employerSupportApiClient.ResendInvitation(data, cancellationToken);

            return new ResendTeamMemberInvitationCommandResult { Success = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ResendTeamMemberInvitationCommandHandler");
            return new ResendTeamMemberInvitationCommandResult { Success = false };
        }
    }
}


using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;

public class ChangeUserRoleCommandHandler(
    IToolsSupportApimService employerSupportApiClient,
    ILogger<ChangeUserRoleCommandHandler> logger)
    : IRequestHandler<ChangeUserRoleCommand, ChangeUserRoleCommandResult>
{
    public async Task<ChangeUserRoleCommandResult> Handle(ChangeUserRoleCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var data = new ChangeUserRoleRequest
            {
                HashedAccountId = command.HashedAccountId,
                Email = command.Email,
                Role = command.Role
            };

            await employerSupportApiClient.ChangeUserRole(data, cancellationToken);

            return new ChangeUserRoleCommandResult { Success = true };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in ChangeUserRoleCommandHandler");
            return new ChangeUserRoleCommandResult { Success = false };
        }
    }
}
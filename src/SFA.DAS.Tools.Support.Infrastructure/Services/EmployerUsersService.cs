using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class EmployerUsersService(
    IEmployerUsersApiClient employerUsersApi,
    ILogger<EmployerUsersService> logger)
    : IEmployerUsersService
{
    
    public async Task<SuspendUserResult> SuspendUser(SuspendUserRequest request, CancellationToken token)
    {
        try
        {
            var result = await employerUsersApi.SuspendUser(request.UserId, new ChangedByUserInfo(request.CurrentUserEmail, request.UserId));

            return new SuspendUserResult
            {
                UserId = result.Id,
                ErrorMessage = result.Errors != null ? string.Join(", ", result.Errors.Select(e => $"{e.Key} {e.Value}")) : null
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to suspend user.");
            return new SuspendUserResult
            {
                ErrorMessage = e.Message
            };
        }
    }

    public async Task<ResumeUserResult> ResumeUser(ResumeUserRequest request, CancellationToken token)
    {
        try
        {
            var result = await employerUsersApi.ResumeUser(request.UserId, new ChangedByUserInfo(request.CurrentUserId, request.CurrentUserEmail));

            return new ResumeUserResult
            {
                UserId = result.Id,
                ErrorMessage = result.Errors != null ? string.Join(", ", result.Errors.Select(e => $"{e.Key} {e.Value}")) : null
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to resume user.");
            return new ResumeUserResult
            {
                ErrorMessage = e.Message
            };
        }
    }
}
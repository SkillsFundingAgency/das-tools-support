using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Responses;

namespace SFA.DAS.Tools.Support.Infrastructure.Services;

public class EmployerUsersService(
    IToolsSupportApimService toolsSupportApimService,
    ILogger<EmployerUsersService> logger)
    : IEmployerUsersService
{
    
    public async Task<SuspendUserResult> SuspendUser(SuspendUserRequest request, CancellationToken token)
    {
        try
        {
            var apiRequest = BuildChangeUserStatusRequest(request.CurrentUserId, request.CurrentUserEmail);
            var result = await toolsSupportApimService.SuspendEmployerUser(request.UserId, apiRequest, token);

            return MapToSuspendResult(result, request.UserId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to suspend user.");
            return new SuspendUserResult
            {
                UserId = request.UserId,
                ErrorMessage = e.Message
            };
        }
    }

    public async Task<ResumeUserResult> ResumeUser(ResumeUserRequest request, CancellationToken token)
    {
        try
        {
            var apiRequest = BuildChangeUserStatusRequest(request.CurrentUserId, request.CurrentUserEmail);
            var result = await toolsSupportApimService.ResumeEmployerUser(request.UserId, apiRequest, token);

            return MapToResumeResult(result, request.UserId);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failure to resume user.");
            return new ResumeUserResult
            {
                UserId = request.UserId,
                ErrorMessage = e.Message
            };
        }
    }

    private static ChangeUserStatusRequest BuildChangeUserStatusRequest(string currentUserId, string currentUserEmail)
    {
        return new ChangeUserStatusRequest
        {
            ChangedByUserId = currentUserId,
            ChangedByEmail = currentUserEmail
        };
    }

    private static SuspendUserResult MapToSuspendResult(ChangeUserStatusResponse response, string userId)
    {
        var errorMessage = BuildErrorMessage(response);
        return new SuspendUserResult
        {
            UserId = response?.Id ?? userId,
            ErrorMessage = errorMessage
        };
    }

    private static ResumeUserResult MapToResumeResult(ChangeUserStatusResponse response, string userId)
    {
        var errorMessage = BuildErrorMessage(response);
        return new ResumeUserResult
        {
            UserId = response?.Id ?? userId,
            ErrorMessage = errorMessage
        };
    }

    private static string BuildErrorMessage(ChangeUserStatusResponse response)
    {
        if (response == null)
        {
            return "Unable to update user status";
        }

        if (response.Errors == null || !response.Errors.Any())
        {
            return null;
        }

        return string.Join(", ", response.Errors.Select(e => $"{e.Key} {e.Value}"));
    }
}
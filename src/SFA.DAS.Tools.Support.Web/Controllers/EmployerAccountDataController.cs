using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.PauseOrResumeApprenticeship))]
public class EmployerAccountDataController(ILogger<EmployerAccountDataController> logger, IEmployerAccountUsersService employerAccountsService)
    : Controller
{
    [HttpPost]
    public async Task<IActionResult> Index(string hashedAccountId, long? internalAccountId)
    {
        if (string.IsNullOrEmpty(hashedAccountId) && !internalAccountId.HasValue)
        {
            return Json(new { ErrorTitle = "Invalid Search", ErrorMessage = "Either the hashed account id or the internal account id must be populated" });
        }

        if (!string.IsNullOrEmpty(hashedAccountId) && internalAccountId.HasValue)
        {
            return Json(new { ErrorTitle = "Invalid Search", ErrorMessage = "Either the hashed account id or the internal account id must be used, not both." });
        }

        var result = await employerAccountsService.GetAccountUsers(new GetAccountUsersRequest
        {
            HashedAccountId = hashedAccountId,
            InternalAccountId = internalAccountId
        });

        if (result.HasError)
        {
            logger.LogError($"Call to Employer Accounts Api Failed with error: {result.ErrorMessage}");
            return Json(new { ErrorTitle = "Call to Employer Accounts Api Failed, please check account ID", ErrorMessage = result.ErrorMessage });
        }

        return Json(result.Users.Select(userDto => new
        {
            userDto.Email,
            userDto.Name,
            userDto.Role,
            userDto.UserRef,
            userDto.AccountStatus,
            userDto.LastSuspendedDate
        }));
    }
}
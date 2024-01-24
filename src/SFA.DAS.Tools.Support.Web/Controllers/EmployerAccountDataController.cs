using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasTier3Account))]
public class EmployerAccountDataController : Controller
{
    private readonly ILogger<EmployerAccountDataController> _logger;
    private readonly IEmployerAccountUsersService _employerAccountsService;

    public EmployerAccountDataController(ILogger<EmployerAccountDataController> logger, IEmployerAccountUsersService employerAccountsService)
    {
        _logger = logger;
        _employerAccountsService = employerAccountsService;
    }

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

        var result = await _employerAccountsService.GetAccountUsers(new GetAccountUsersRequest
        {
            HashedAccountId = hashedAccountId,
            InternalAccountId = internalAccountId
        }, new CancellationToken());

        if (result.HasError)
        {
            _logger.LogError($"Call to Employer Accounts Api Failed with error: {result.ErrorMessage}");
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class EmployerAccountDataController : Controller
    {
        private readonly ILogger<EmployerAccountDataController> _logger;
        private readonly IEmployerAccountsService _employerAccountsService;

        public EmployerAccountDataController(ILogger<EmployerAccountDataController> logger, IEmployerAccountsService employerAccountsService)
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

            var result = await _employerAccountsService.GetAccountUsers(new Core.Models.GetAccountUsersRequest
            {
                HashedAccountId = hashedAccountId,
                InternalAccountId = internalAccountId
            }, new CancellationToken());

            if(result.HasError)
            {
                _logger.LogError($"Call to Employer Accounts Api Failed with error: {result.ErrorMessage}");
                return Json(new { ErrorTitle = "Call to Employer Accounts Api Failed, please check account ID", ErrorMessage = result.ErrorMessage });
            }

            return Json(result.Users.Select(u => new 
            {
                u.Email,
                u.Name,
                u.Role,                
                u.UserRef
            }));
        }
    }
}

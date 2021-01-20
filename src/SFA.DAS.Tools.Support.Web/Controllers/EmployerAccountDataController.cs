using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
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
        public async Task<IActionResult> Index(string accountId)
        {
            if (string.IsNullOrEmpty(accountId))
            {
                return Json(new { ErrorTitle = "Invalid Search", ErrorMessage = "Account Id must be populated" });
            }

            var result = await _employerAccountsService.GetAccountUsers(new Core.Models.GetAccountUsersRequest
            {
                AccountId = accountId
            }, new CancellationToken());

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

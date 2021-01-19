using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IEmployerAccountsService _accountsService;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IEmployerAccountsService accountsService)
        {
            _accountsService = accountsService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string accountId)
        {
            var result = await _accountsService.GetAccountUsers(new Core.Models.GetAccountUsersRequest
            {
                AccountId = accountId
            }, new CancellationToken());

            return View(new UserViewModel()
            {
                Users = result.Users
            });
        }
    }
}
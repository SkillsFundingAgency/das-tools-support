using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class UserSearchController : Controller
    {
        private readonly IEmployerAccountsService _accountsService;
        private readonly ILogger<UserSearchController> _logger;

        public UserSearchController(ILogger<UserSearchController> logger, IEmployerAccountsService accountsService)
        {
            _accountsService = accountsService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string accountId)
        {
            return View(new UserViewModel() { AccountId = accountId });
        }       
    }
}
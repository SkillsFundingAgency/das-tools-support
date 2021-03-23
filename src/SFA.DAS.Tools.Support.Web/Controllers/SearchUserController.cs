using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/searchuser")]
    public class SearchUserController : Controller
    {
        public SearchUserController(ILogger<SearchUserController> logger, IEmployerAccountsService accountsService)
        {
        }

        [HttpGet]
        public IActionResult Index(string hashedAccountId, long? internalAccountId, string act)
        {
            switch(act)
            {
                case ActionNames.Suspend:
                    ViewData.Add("FormActionRoute", RouteNames.SuspendUsers);
                    ViewData.Add("FormActionText", "Suspend user(s)");
                break;
                case ActionNames.Resume:
                    ViewData.Add("FormActionRoute", RouteNames.ResumeUsers);
                    ViewData.Add("FormActionText", "Reinstate user(s)");
                break;
                default:
                    return BadRequest();
            }
            
            return View(new UserViewModel() { HashedAccountId = hashedAccountId, InternalAccountId = internalAccountId });
        }
    }
}
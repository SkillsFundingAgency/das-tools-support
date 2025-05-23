using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("support/searchuser")]
[Authorize(Policy = nameof(PolicyNames.PauseOrResumeApprenticeship))]
public class SearchUserController : Controller
{
    public SearchUserController(ILogger<SearchUserController> logger, IEmployerAccountUsersService accountsService)
    {
    }

    [HttpGet]
    public IActionResult Index(string hashedAccountId, long? internalAccountId, string act)
    {
        switch (act)
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

        return View(new UserViewModel { HashedAccountId = hashedAccountId, InternalAccountId = internalAccountId });
    }
}
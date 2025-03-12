using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class SupportController(IAuthorizationProvider authorizationProvider, ToolsSupportConfig toolsSupportConfig) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (toolsSupportConfig.EnableSupportConsoleFeature)
        {
            var isEmployerSupportOnlyAuthorized = await authorizationProvider.IsEmployerSupportOnlyAuthorized(User);

            if (isEmployerSupportOnlyAuthorized)
            {
                return RedirectToAction("Index", "EmployerSupport");
            }
        }

        var isPauseOrResumeApprenticeshipAuthorized = await authorizationProvider.IsPauseOrResumeApprenticeshipAuthorized(User);

        if (!isPauseOrResumeApprenticeshipAuthorized)
        {
            RedirectToAction("Index", "Home");
        }
        
        var isEmployerSupportAuthorized = await authorizationProvider.IsEmployerSupportAuthorized(User);

        var indexViewModel = new IndexViewModel
        {
            HasTier3Account = isPauseOrResumeApprenticeshipAuthorized,
            HasEmployerSupportAccount = isEmployerSupportAuthorized
        };

        return View(indexViewModel);
    }

    [AllowAnonymous]
    public IActionResult LoggedOut()
    {
        return View();
    }
}
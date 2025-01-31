using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class SupportController(IAuthorizationProvider authorizationProvider) : Controller
{
    public async Task<IActionResult> Index()
    {
        var isPrivilegeAuthorized = await authorizationProvider.IsPrivilegeAuthorized(User);

        if (!isPrivilegeAuthorized)
        {
            RedirectToAction("Index", "Home");
        }
        
        var indexViewModel = new IndexViewModel
        {
            HasTier3Account = isPrivilegeAuthorized
        };

        return View(indexViewModel);
    }

    [AllowAnonymous]
    public IActionResult LoggedOut()
    {
        return View();
    }
}
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class SupportController(IAuthorizationService authorizationService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var tier3Authorization = await authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier3Account));

        if (!tier3Authorization.Succeeded)
        {
            RedirectToAction("Index", "Home");
        }
        
        var tier1Authorization = await authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier1Account));
        var tier2Authorization = await authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier2Account));
       
        var indexViewModel = new IndexViewModel
        {
            HasSupportConsoleAccess = tier1Authorization.Succeeded || tier2Authorization.Succeeded,
            HasTier3Account = tier3Authorization.Succeeded
        };

        return View(indexViewModel);
    }

    [AllowAnonymous]
    public IActionResult LoggedOut()
    {
        return View();
    }
}
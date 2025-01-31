using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class SupportController(IAuthorizationProvider authorizationService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var isTier3Authorized = await authorizationService.IsTier3Authorized(User);

        if (!isTier3Authorized)
        {
            RedirectToAction("Index", "Home");
        }
        
        var isTier1Authorized = await authorizationService.IsTier1Authorized(User);
        var isTier2Authorized = await authorizationService.IsTier2Authorized(User);
       
        var indexViewModel = new IndexViewModel
        {
            HasSupportConsoleAccess = isTier1Authorized || isTier2Authorized,
            HasTier3Account = isTier3Authorized
        };

        return View(indexViewModel);
    }

    [AllowAnonymous]
    public IActionResult LoggedOut()
    {
        return View();
    }
}
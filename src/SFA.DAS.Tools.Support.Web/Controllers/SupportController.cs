using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class SupportController(IAuthorizationProvider authorizationProvider, ILogger<SupportController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (User != null && User.Claims.Any())
        {
            logger.LogInformation("SupportController-Index userClaims: {Data}", JsonSerializer.Serialize(User.Claims.Select(x => new
            {
                x.Type,
                x.Value
            })));
        }

        var isTier3Authorized = await authorizationProvider.IsTier3Authorized(User);

        if (!isTier3Authorized)
        {
            RedirectToAction("Index", "Home");
        }

        var isTier1Authorized = await authorizationProvider.IsTier1Authorized(User);
        var isTier2Authorized = await authorizationProvider.IsTier2Authorized(User);

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
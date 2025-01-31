using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.Home;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[AllowAnonymous]
public class HomeController(
    ToolsSupportConfig toolsSupportConfig,
    IAuthorizationService authorizationService) : Controller
{
    public async Task<IActionResult> Index()
    {
        // if the user is already signed in, then redirect the user to the support home page.
        if (!toolsSupportConfig.UseDfESignIn || User.Identity is not { IsAuthenticated: true })
        {
            return View(new HomeIndexViewModel
            {
                UseDfESignIn = toolsSupportConfig.UseDfESignIn
            });
        }

        var tier3Authorization = await authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier3Account));

        if (tier3Authorization.Succeeded || !toolsSupportConfig.EnableSupportConsoleFeature)
        {
            return RedirectToAction("Index", "Support");    
        }

        return RedirectToAction("Index", "EmployerSupport");

    }

    [HttpGet("~/signout", Name = RouteNames.SignOut)]
    public IActionResult SignOut()
    {
        var authScheme = toolsSupportConfig.UseDfESignIn
            ? OpenIdConnectDefaults.AuthenticationScheme
            : WsFederationDefaults.AuthenticationScheme;

        return SignOut(new AuthenticationProperties
        {
            RedirectUri = "",
            AllowRefresh = true
        }, CookieAuthenticationDefaults.AuthenticationScheme, authScheme);
    }
}
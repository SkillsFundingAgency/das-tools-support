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
    IAuthorizationProvider authorizationProvider) : Controller
{
    public async Task<IActionResult> Index()
    {
        // Display index view if UseDfeSignIn is false or if user is not-authenticated.
        if (!toolsSupportConfig.UseDfESignIn || User.Identity is not { IsAuthenticated: true })
        {
            return View(new HomeIndexViewModel
            {
                UseDfESignIn = toolsSupportConfig.UseDfESignIn
            });
        }

        if (!toolsSupportConfig.EnableSupportConsoleFeature)
        {
            return RedirectToAction("Index", "Support");
        }

        var isEmployerSupportOnlyAuthorized = await authorizationProvider.IsEmployerSupportOnlyAuthorized(User);

        return RedirectToAction("Index", isEmployerSupportOnlyAuthorized ? "EmployerSupport" : "Support");
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
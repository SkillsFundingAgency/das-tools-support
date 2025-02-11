using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[AllowAnonymous]
public class HomeController(
    ToolsSupportConfig toolsSupportConfig,
    IAuthorizationProvider authorizationProvider) : Controller
{
    public async Task<IActionResult> Index()
    {
        // if the user is already signed in, then redirect the user to the support home page.
        if (User.Identity is not { IsAuthenticated: true })
        {
            return View();
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
        return SignOut(new AuthenticationProperties
        {
            RedirectUri = "",
            AllowRefresh = true
        }, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }
}
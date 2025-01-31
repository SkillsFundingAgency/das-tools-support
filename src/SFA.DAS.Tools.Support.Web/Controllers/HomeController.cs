using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.Home;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[AllowAnonymous]
public class HomeController(ToolsSupportConfig toolsSupportConfig) : Controller
{
    public IActionResult Index()
    {
        // if the user is already signed in, then redirect the user to the support home page.
        if(toolsSupportConfig.UseDfESignIn && User.Identity is {IsAuthenticated: true}) return RedirectToAction("Index", "Support");

        return View(new HomeIndexViewModel
        {
            UseDfESignIn = toolsSupportConfig.UseDfESignIn
        });
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
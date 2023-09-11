﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.Home;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly DfESignInConfig _dfESignInOptions;

        public HomeController(IOptions<DfESignInConfig> dfESignInOptions)
        {
            _dfESignInOptions = dfESignInOptions.Value;
        }

        public IActionResult Index()
        {
            return View(new HomeIndexViewModel
            {
                UseDfESignIn = _dfESignInOptions.UseDfESignIn
            });
        }

        [HttpGet("~/signout", Name = RouteNames.SignOut)]
        public IActionResult SignOut()
        {
            var authScheme = _dfESignInOptions.UseDfESignIn
                ? OpenIdConnectDefaults.AuthenticationScheme
                : WsFederationDefaults.AuthenticationScheme;

            return SignOut(new AuthenticationProperties
            {
                RedirectUri = "",
                AllowRefresh = true
            }, CookieAuthenticationDefaults.AuthenticationScheme, authScheme);
        }
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class SupportController : Controller
    {
        private readonly ILogger<SupportController> _logger;

        public SupportController(ILogger<SupportController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("support/logout")]
        public IActionResult LogOut()
        {           
            return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties 
            {
                RedirectUri = "https://localhost:5021/Support/LoggedOut"
            }, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [Route("support/loggedout")]
        public IActionResult LoggedOut()
        {
            return View();
        }
    }
}

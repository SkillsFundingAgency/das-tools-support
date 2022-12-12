using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class SupportController : Controller
    {
        private readonly string _baseUrl;
        private IAuthorizationService _authorizationService;

        public SupportController(ILogger<SupportController> logger, 
            IConfiguration _configuration,
            IAuthorizationService authorizationService)
        {
            var baseUrl = _configuration.GetValue<string>("BaseUrl");
            if (!baseUrl.EndsWith('/'))
            {
                _baseUrl = string.Concat(baseUrl, '/');
            }
            else
            {
                _baseUrl = baseUrl;
            }

            _authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            var indexViewModel = new IndexViewModel()
            {
                HasTier3Account = _authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier3Account)).Result.Succeeded
            };

            return View(indexViewModel);
        }

        [AllowAnonymous]
        public IActionResult LogOut()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties
            {
                RedirectUri = $"{_baseUrl}Support/LoggedOut"
            }, CookieAuthenticationDefaults.AuthenticationScheme, WsFederationDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public IActionResult LoggedOut()
        {
            return View();
        }
    }
}

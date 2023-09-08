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
    }
}

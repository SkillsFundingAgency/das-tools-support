using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.Error;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [AllowAnonymous]
    [Route("/error")]
    public class ErrorController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DfESignInConfig _dfESignInOptions;

        public ErrorController(
            IOptions<DfESignInConfig> dfESignInOptions,
            IConfiguration configuration)
        {
            _configuration = configuration;
            _dfESignInOptions = dfESignInOptions.Value;
        }

        [Route("403")]
        public IActionResult AccessDenied()
        {
            return View(new Error403ViewModel(_configuration["ResourceEnvironmentName"]) { UseDfESignIn = _dfESignInOptions.UseDfESignIn });
        }

        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("500")]
        public IActionResult ApplicationError()
        {
            return View();
        }
    }
}

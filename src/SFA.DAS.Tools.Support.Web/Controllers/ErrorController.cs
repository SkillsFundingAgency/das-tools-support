using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("/error")]
    public class ErrorController : BaseController<ErrorController>
    {
        [Route("403")]
        public IActionResult AccessDenied()
        {
            return View();
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

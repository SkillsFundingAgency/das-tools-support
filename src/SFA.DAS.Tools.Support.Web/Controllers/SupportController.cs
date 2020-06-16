using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Authorize]
    public class SupportController : BaseController<SupportController>
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
    }
}

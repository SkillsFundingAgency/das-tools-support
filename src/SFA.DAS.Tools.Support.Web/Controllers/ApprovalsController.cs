using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Handlers;
using SFA.DAS.Tools.Support.Core.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("Approvals")]
    public class ApprovalsController : Controller
    {
        private readonly ILogger<ApprovalsController> _logger;
        private readonly IMediatorService _mediator;

        public ApprovalsController(ILogger<ApprovalsController> logger, IMediatorService mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("stopApprenticeship", Name =ApprovalsRouteNames.StopApprenticeship)]
        public IActionResult StopApprenticeship()
        {
            return View();
        }

        [HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(StopApprenticeshipViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid Model State");
                return View(model);
            }

            var result = await _mediator.Send<StopApprenticeshipMessage, StopApprenticeshipMessageResult>(new StopApprenticeshipMessage
            {
                ApprenticeshipId = model.ApprenticeshipId,
                EmployerAccountId = model.EmployerAccountId,
                StopDate = model.StopDate
            });

            if(!result.HasSuccess)
            {
                ModelState.AddModelError("", $"Call to Commitments Api Failed {result.ErrorMessage}");
                return View(model);   
            }

            return RedirectToAction(ApprovalsRouteNames.StopApprenticeshipSubmitted);
        }

        [HttpGet("StopApprenticeshipSubmitted", Name = ApprovalsRouteNames.StopApprenticeshipSubmitted)]
        public IActionResult StopApprenticeshipSubmitted()
        {
            return View();
        }
    }
}

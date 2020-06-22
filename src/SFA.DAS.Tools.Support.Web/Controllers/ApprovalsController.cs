using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Handlers;
using SFA.DAS.Tools.Support.Core.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("Approvals")]
    public class ApprovalsController : Controller
    {
        private readonly ILogger<ApprovalsController> _logger;
        private readonly IMediatorService _mediator;
        private readonly string claimUserName = "https://tools.apprenticeships.education.gov.uk/claims/nickname";

        public ApprovalsController(ILogger<ApprovalsController> logger, IMediatorService mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
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

            var userId = HttpContext.User.Claims.FirstOrDefault(s => s.Type == claimUserName)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                ModelState.AddModelError("", "Unable to retrieve username from claim for request to Stop Apprenticeship");
                return View(model);
            }

            var result = await _mediator.Send<StopApprenticeshipMessage, StopApprenticeshipMessageResult>(new StopApprenticeshipMessage
            {
                ApprenticeshipId = model.ApprenticeshipId,
                EmployerAccountId = model.EmployerAccountId,
                StopDate = model.StopDate,
                UserId = userId
            });

            if (!result.HasSuccess)
            {
                ModelState.AddModelError("", $"Call to Commitments Api Failed {result.ErrorMessage}");
                return View(model);
            }
            else
            {
                model.SubmittedSuccessfully = result.HasSuccess;
                return View(model);
            }
        }
    }
}

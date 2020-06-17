using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Handlers;
using SFA.DAS.Tools.Support.Core.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class ApprovalsController : Controller
    {
        private readonly ILogger<ApprovalsController> _logger;
        private readonly IMediatorService _mediator;

        public ApprovalsController(ILogger<ApprovalsController> logger, IMediatorService mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public IActionResult StopApprenticeship()
        {
            return View();
        }

        [HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.SubmitStopApprenticeship)]
        public async Task<IActionResult> SubmitStopApprenticeship(StopApprenticeshipViewModel model)
        {
            // Add in validation for state of fields
            // add fields on form to supply the information
            // add in configuration to call commitments api.
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid Model State");
                return View("Index", model);
            }

            await _mediator.Publish(new StopApprenticeshipMessage
            {
                ApprenticeshipId = model.ApprenticeshipId,
                EmployerAccountId = model.EmployerAccountId,
                StopDate = model.StopDate
            });

            return View("StopApprenticeship", model);
        }
    }
}

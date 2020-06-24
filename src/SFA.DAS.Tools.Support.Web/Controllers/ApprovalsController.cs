using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("approvals")]
    public class ApprovalsController : Controller
    {
        private readonly ILogger<ApprovalsController> _logger;
        private readonly IEmployerCommitmentsService _employerCommitmentsService;
        private readonly IMapper _mapper;
        private readonly string claimUserName = "https://tools.apprenticeships.education.gov.uk/claims/nickname";

        public ApprovalsController(ILogger<ApprovalsController> logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper)
        {
            _logger = logger;
            _employerCommitmentsService = employerCommitmentsService;
            _mapper = mapper;
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

            var result = await _employerCommitmentsService.GetApprenticeshipSummary(model.ApprenticeshipId, model.EmployerAccountId);

            if (result.HasError)
            {
                ModelState.AddModelError("", $"Call to Commitments Api Failed {result.ErrorMessage}");
                return View(model);
            }
            else
            {
                var confirmation = _mapper.Map<StopApprenticeshipConfirmationViewModel>(result);

                confirmation.ApprenticeshipId = model.ApprenticeshipId;
                confirmation.EmployerAccountId = model.EmployerAccountId;
                confirmation.EnteredStopDate = model.StopDate;

                return View("StopApprenticeshipConfirmation", confirmation);
            }
        }

        [HttpPost("stopApprenticeshipConfirmation", Name = ApprovalsRouteNames.StopApprenticeshipConfirmation)]
        public async Task<IActionResult> StopApprenticeshipConfirmation(StopApprenticeshipConfirmationViewModel model)
        {

            var userId = HttpContext.User.Claims.FirstOrDefault(s => s.Type == claimUserName)?.Value;
            var result = await _employerCommitmentsService.StopApprenticeship(model.EmployerAccountId, model.ApprenticeshipId, userId, model.EnteredStopDate);

            if (result.HasError)
            {
                ModelState.AddModelError("", $"Call to Commitments Api Failed {result.ErrorMessage}");
                return View(model);
            }
            else
            {
                model.SubmittedSuccessfully = true;
                return View(model);
            }
        }
    }
}

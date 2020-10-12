using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/approvals")]
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

        [HttpGet("searchApprenticeships", Name = ApprovalsRouteNames.SearchApprenticeships)]
        public IActionResult SearchApprenticeships()
        {
            return View();
        }

        [HttpPost("searchApprenticeships", Name = ApprovalsRouteNames.SearchApprenticeships)]
        public async Task<IActionResult> SearchApprenticeships(SearchApprenticeshipsViewModel model)
        {
            if (model.IsModelEmpty)
            {
                // throw invalid json result
                return Json(new { ErrorTitle = "Invalid Search", ErrorMessage = "At least one parameter must be populated" });
            }

            var result = await _employerCommitmentsService.SearchApprenticeships(
                model.CourseName,
                model.EmployerName,
                model.ProviderName,
                model.SearchTerm,
                model.StartDate,
                model.EndDate);

            if (result.HasError)
            {
                return Json(new { ErrorTitle = "Call to Commitments Api Failed", ErrorMessage = result.ErrorMessage });
            }
            else
            {
                return Json(result.Apprenticeships.Select(a => new
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    EmployerName = a.EmployerName,
                    ProviderName = a.ProviderName,
                    StartDate = a.StartDate.ToShortDateString(),
                    EndDate = a.EndDate.ToShortDateString(),
                    Status = a.ApprenticeshipStatus.ToString()
                }));
            }
        }

        [HttpGet("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship([FromQuery] long id)
        {
            var result = await _employerCommitmentsService.GetApprenticeship(id);

            if(result.HasError)
            {
                // sort out error page
                return BadRequest();
            } 

            var stopApprenticeshipModel = _mapper.Map<StopApprenticeshipViewModel>(result.Apprenticeship);
            return View(stopApprenticeshipModel);
        }

        [HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(StopApprenticeshipViewModel model, string submit)
        {
            if (!string.IsNullOrWhiteSpace(submit) && submit.Equals("cancel", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return View("SearchApprenticeships");
            }

            if(model.EnteredStopDate < DateTime.Today)
            {
                ModelState.AddModelError("Stop Date Invalid", "Stop date must be greater than or equal to today.");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid Model State");
                var stopApprenticeshipModel = await RepopulateApprenticeshipModel(model.ApprenticeshipId, model.EnteredStopDate);
                return View(stopApprenticeshipModel);
            }

            var userId = HttpContext.User.Claims.FirstOrDefault(s => s.Type == claimUserName)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                var stopApprenticeshipModel = await RepopulateApprenticeshipModel(model.ApprenticeshipId, model.EnteredStopDate);
                ModelState.AddModelError("", "Unable to retrieve username from claim for request to Stop Apprenticeship");
                return View(stopApprenticeshipModel);
            }

            var result = await _employerCommitmentsService.StopApprenticeship(model.EmployerAccountId, model.ApprenticeshipId, userId, model.EnteredStopDate);

            if (result.HasError)
            {
                ModelState.AddModelError("", $"Call to Commitments Api Failed {result.ErrorMessage}");
                var stopApprenticeshipModel = await RepopulateApprenticeshipModel(model.ApprenticeshipId, model.EnteredStopDate);
                return View(stopApprenticeshipModel);
            }
            else
            {
                model.SubmittedSuccessfully = true;
                return View(model);
            }
        }

        private async Task<StopApprenticeshipViewModel> RepopulateApprenticeshipModel(long id, DateTime stopDate)
        {
            var apprenticeshipResponse = await _employerCommitmentsService.GetApprenticeship(id);
            var stopApprenticeshipModel = _mapper.Map<StopApprenticeshipViewModel>(apprenticeshipResponse.Apprenticeship);
            stopApprenticeshipModel.EnteredStopDate = stopDate;
            return stopApprenticeshipModel;
        }
    }
}

﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
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
        public IActionResult StopApprenticeship([FromQuery] long id)
        {
            // search apprenticeships using
            //var result = await _employerCommitmentsService.SearchApprenticeships();
            // add in material design
            // add in table view to select a given apprenticeship
            // Validate this side to confirm it can be stopped? or allow api to do it
            // Then show given error message.
            // 
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
        public async Task<IActionResult> StopApprenticeshipConfirmation(StopApprenticeshipConfirmationViewModel model, string submit)
        {
            if (!string.IsNullOrWhiteSpace(submit) && submit.Equals("cancel", System.StringComparison.InvariantCultureIgnoreCase))
            {
                var stopModel = new StopApprenticeshipViewModel
                {
                    ApprenticeshipId = model.ApprenticeshipId,
                    EmployerAccountId = model.EmployerAccountId,
                    StopDate = model.EnteredStopDate
                };

                return View("StopApprenticeship", stopModel);
            }

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

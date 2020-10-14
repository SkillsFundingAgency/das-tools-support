using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/approvals")]
    public class ApprovalsController : Controller
    {
        private readonly ILogger<ApprovalsController> _logger;
        private readonly IEmployerCommitmentsService _employerCommitmentsService;
        private readonly IMapper _mapper;
        private readonly string emailClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        private readonly string nameClaim = "name";
        private readonly string userIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";

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
                new SearchApprenticeshipsRequest
                {
                    CourseName = model.CourseName,
                    EmployerName = model.EmployerName,
                    ProviderName = model.ProviderName,
                    SearchTerm = model.SearchTerm,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                },new CancellationToken());

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
            var result = await _employerCommitmentsService.GetApprenticeship(id, new CancellationToken());

            if (result.HasError)
            {
                var model = new StopApprenticeshipViewModel
                {
                    ApprenticeshipNotFound = true,
                    ApprenticeshipId = id
                };
                return View(model);
            }

            var stopApprenticeshipModel = _mapper.Map<StopApprenticeshipViewModel>(result.Apprenticeship);
            stopApprenticeshipModel.EnteredStopDate = DateTime.Today;
            return View(stopApprenticeshipModel);
        }

        [HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(StopApprenticeshipViewModel model, string submit)
        {
            if (!string.IsNullOrWhiteSpace(submit) && submit.Equals("cancel", System.StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToAction(ApprovalsRouteNames.SearchApprenticeships);
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid Model State");
                return View(model);
            }

            var userEmail = HttpContext.User.Claims.FirstOrDefault(s => s.Type == emailClaim)?.Value;
            var userId = HttpContext.User.Claims.FirstOrDefault(s => s.Type == userIdClaim)?.Value;
            var displayName = HttpContext.User.Claims.FirstOrDefault(s => s.Type == nameClaim)?.Value;

            if (string.IsNullOrWhiteSpace(userEmail) && string.IsNullOrWhiteSpace(displayName))
            {
                ModelState.AddModelError("", "Unable to retrieve email or name from claim for request to Stop Apprenticeship");
                return View(model);
            }

            var result = await _employerCommitmentsService.StopApprenticeship(new StopApprenticeshipRequest
            {
                AccountId = model.EmployerAccountId,
                ApprenticeshipId = model.ApprenticeshipId,
                StopDate = model.EnteredStopDate,
                MadeRedundant = false,
                DisplayName = displayName,
                EmailAddress = userEmail,
                UserId = userId
            }, new CancellationToken());

            if (result.HasError)
            {
                ModelState.AddModelError("", $"Call to Commitments Api Failed: {result.ErrorMessage}");
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

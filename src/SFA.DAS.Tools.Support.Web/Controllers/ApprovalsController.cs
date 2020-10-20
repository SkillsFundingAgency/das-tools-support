using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static SFA.DAS.Tools.Support.Web.Models.StopApprenticeshipViewModel;

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
        public IActionResult SearchApprenticeships(string employerName, string courseName, string providerName, string apprenticeName, DateTime? startDate, DateTime? endDate, string selectedStatus)
        {
            // To Do Tuesday
            // then look at stopping the apprenticeships
            // then what happens if there is an error? :thinking face
            // bulk error handling
            // unit tests
            // UI Improvemeng

            var sDate = startDate.HasValue && startDate.Value != DateTime.MinValue ? startDate : null;
            var eDate = endDate.HasValue && endDate.Value != DateTime.MinValue ? endDate : null;
            var status = string.IsNullOrWhiteSpace(selectedStatus) ? "0" : selectedStatus;

            var model = new SearchApprenticeshipsViewModel
            {
                EmployerName = employerName,
                CourseName = courseName,
                ProviderName = providerName,
                StartDate = sDate,
                EndDate = eDate,
                SelectedStatus = status,
                ApprenticeName = apprenticeName
            };

            return View(model);
        }

        [HttpPost("searchApprenticeships", Name = ApprovalsRouteNames.SearchApprenticeships)]
        public async Task<IActionResult> SearchApprenticeships(SearchApprenticeshipsViewModel model)
        {
            if (model.IsModelEmpty)
            {
                return Json(new { ErrorTitle = "Invalid Search", ErrorMessage = "At least one parameter must be populated" });
            }

            var result = await _employerCommitmentsService.SearchApprenticeships(
                new SearchApprenticeshipsRequest
                {
                    CourseName = model.CourseName,
                    EmployerName = model.EmployerName,
                    ProviderName = model.ProviderName,
                    SearchTerm = model.ApprenticeName,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ApprenticeshipStatus = model.SelectedStatus
                }, new CancellationToken());

            if (result.HasError)
            {
                return Json(new { ErrorTitle = "Call to Commitments Api Failed", ErrorMessage = result.ErrorMessage });
            }

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

        [HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(ApprenticeshipSearchResultsViewModel model)
        {
            // what if empty? 
            var tasks = new List<Task<GetApprenticeshipResult>>();
            foreach (var id in model.SelectedIds.Split(','))
            {
                if (int.TryParse(id, out var longId))
                {
                    tasks.Add(_employerCommitmentsService.GetApprenticeship(longId, new CancellationToken()));
                }
            }

            var results = await Task.WhenAll(tasks);

            if (results.Any(a => a.HasError))
            {
                var stopModelError = new StopApprenticeshipViewModel
                {
                    HasError = true
                };
                return View(stopModelError);
            }
            
            // Reconstruct Search Params for return to search page.
            var searchParams = new SearchParameters
            {
                ApprenticeName = model.ApprenticeName,
                CourseName = model.CourseName,
                EmployerName = model.EmployerName,
                ProviderName = model.ProviderName,
                SelectedStatus = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            return View(new StopApprenticeshipViewModel { Apprenticeships = results.Select(s => s.Apprenticeship), SearchParams = searchParams });
        }

        [HttpPost("cancelStopApprenticeship", Name = ApprovalsRouteNames.CancelStopApprenticeship)]
        public IActionResult CancelStopApprenticeship(StopApprenticeshipViewModel model)
        {
            return RedirectToAction(ApprovalsRouteNames.SearchApprenticeships, new
            {
                model.SearchParams.ApprenticeName,
                model.SearchParams.CourseName,
                model.SearchParams.ProviderName,
                model.SearchParams.EmployerName,
                model.SearchParams.SelectedStatus,
                EndDate = model.SearchParams.GetFormattedEndDate,
                StartDate = model.SearchParams.GetFormattedStartDate
            });
        }

        [HttpPost("stopApprenticeshipConfirmation", Name = ApprovalsRouteNames.StopApprenticeshipConfirmation)]
        public async Task<IActionResult> StopApprenticeshipConfirmation(StopApprenticeshipViewModel model)
        {
            // what if empty?
            // need to deserialize back into wht was created ( maybe create a class for this for ease? )

            var collection = JsonSerializer.Deserialize<List<ApprenticeshipDto>>(model.ApprenticeshipsData);
            return Ok();
        }

        //[HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        //public async Task<IActionResult> StopApprenticeship(StopApprenticeshipViewModel model, string submit)
        //{
        //    if (!string.IsNullOrWhiteSpace(submit) && submit.Equals("cancel", System.StringComparison.InvariantCultureIgnoreCase))
        //    {
        //        return RedirectToAction(ApprovalsRouteNames.SearchApprenticeships);
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogError("Invalid Model State");
        //        return View(model);
        //    }

        //    var userEmail = HttpContext.User.Claims.FirstOrDefault(s => s.Type == emailClaim)?.Value;
        //    var userId = HttpContext.User.Claims.FirstOrDefault(s => s.Type == userIdClaim)?.Value;
        //    var displayName = HttpContext.User.Claims.FirstOrDefault(s => s.Type == nameClaim)?.Value;

        //    if (string.IsNullOrWhiteSpace(userEmail) && string.IsNullOrWhiteSpace(displayName))
        //    {
        //        ModelState.AddModelError("", "Unable to retrieve email or name from claim for request to Stop Apprenticeship");
        //        return View(model);
        //    }

        //    var result = await _employerCommitmentsService.StopApprenticeship(new StopApprenticeshipRequest
        //    {
        //        AccountId = model.EmployerAccountId,
        //        ApprenticeshipId = model.ApprenticeshipId,
        //        StopDate = model.EnteredStopDate,
        //        MadeRedundant = false,
        //        DisplayName = displayName,
        //        EmailAddress = userEmail,
        //        UserId = userId
        //    }, new CancellationToken());

        //    if (result.HasError)
        //    {
        //        ModelState.AddModelError("", $"Call to Commitments Api Failed: {result.ErrorMessage}");
        //        return View(model);
        //    }
        //    else
        //    {
        //        model.SubmittedSuccessfully = true;
        //        return View(model);
        //    }
        //}
    }
}

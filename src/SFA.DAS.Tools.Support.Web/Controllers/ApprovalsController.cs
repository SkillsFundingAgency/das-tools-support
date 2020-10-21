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

            return Json(result.Apprenticeships.Select(a => new StopApprenticeshipRow
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                EmployerName = a.EmployerName,
                ProviderName = a.ProviderName,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Status = a.ApprenticeshipStatus.ToString(),
                PaymentStatus = a.PaymentStatus.ToString(),
            }));
        }

        [HttpPost("stopApprenticeship", Name = ApprovalsRouteNames.StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(ApprenticeshipSearchResultsViewModel model)
        {
            var tasks = new List<Task<GetApprenticeshipResult>>();
            var ids = model.SelectedIds.Split(',');

            if(ids.Count() == 0)
            {
                return RedirectToAction(ApprovalsRouteNames.SearchApprenticeships, new
                {
                    model.ApprenticeName,
                    model.CourseName,
                    model.ProviderName,
                    model.EmployerName,
                    SelectedStatus = model.Status,
                    EndDate = model.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd"),
                    StartDate = model.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd")
                });
            }

            foreach (var id in ids)
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

            return View(new StopApprenticeshipViewModel { Apprenticeships = _mapper.Map<List<StopApprenticeshipRow>>(results.Select(s => s.Apprenticeship)), SearchParams = searchParams });
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
            var userEmail = HttpContext.User.Claims.FirstOrDefault(s => s.Type == emailClaim)?.Value;
            var userId = HttpContext.User.Claims.FirstOrDefault(s => s.Type == userIdClaim)?.Value;
            var displayName = HttpContext.User.Claims.FirstOrDefault(s => s.Type == nameClaim)?.Value;
            var apprenticeshipsData = JsonSerializer.Deserialize<List<StopApprenticeshipRow>>(model.ApprenticeshipsData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            if (string.IsNullOrWhiteSpace(userEmail) && string.IsNullOrWhiteSpace(displayName))
            {
                model.Apprenticeships = apprenticeshipsData;
                ModelState.AddModelError("", "Unable to retrieve email or name from claim for request to Stop Apprenticeship");
                return View("StopApprenticeship", model);
            }

            if (apprenticeshipsData.Any(s => s.GetStopDate == null && s.ApiSubmissionStatus != StopApprenticeshipRow.SubmissionStatus.Successful))
            {
                model.Apprenticeships = apprenticeshipsData;
                ModelState.AddModelError("", "Not all Apprenticeship rows have been supplied with a stop date.");
                return View("StopApprenticeship", model);
            }

            var stopApprenticeshipTasks = new List<Task<StopApprenticeshipResult>>();
            foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != StopApprenticeshipRow.SubmissionStatus.Successful))
            {
                stopApprenticeshipTasks.Add(_employerCommitmentsService.StopApprenticeship(new Core.Models.StopApprenticeshipRequest
                {
                    AccountId = apprenticeship.AccountId,
                    ApprenticeshipId = apprenticeship.Id,
                    StopDate = apprenticeship.GetStopDate.Value,
                    MadeRedundant = false,
                    DisplayName = displayName,
                    EmailAddress = userEmail,
                    UserId = userId
                }, new CancellationToken()));
            }

            var results = await Task.WhenAll(stopApprenticeshipTasks);

            foreach (var apprenticeship in apprenticeshipsData)
            {
                var result = results.Where(s => s.ApprenticeshipId == apprenticeship.Id).FirstOrDefault();
                if (result == null)
                {
                    continue;
                }

                if (!result.HasError)
                {
                    apprenticeship.ApiSubmissionStatus = StopApprenticeshipRow.SubmissionStatus.Successful;
                    apprenticeship.ApiErrorMessage = string.Empty;
                }
                else
                {
                    apprenticeship.ApiSubmissionStatus = StopApprenticeshipRow.SubmissionStatus.Errored;
                    apprenticeship.ApiErrorMessage = result.ErrorMessage;
                }
            }

            model.Apprenticeships = apprenticeshipsData;
            return View("StopApprenticeship", model);
        }
    }
}

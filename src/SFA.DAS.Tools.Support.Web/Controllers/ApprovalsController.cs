using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
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
    public class ApprovalsController : ApprovalsControllerBase
    {
         public ApprovalsController(ILogger<ApprovalsController> logger,
            IEmployerCommitmentsService employerCommitmentsService,
            IMapper mapper,
            IOptions<ClaimsConfiguration> claimConfiguration) :
            base(logger, employerCommitmentsService, mapper, claimConfiguration)
        {
        }

        [HttpGet("searchApprenticeships", Name = RouteNames.Approval_SearchApprenticeships)]
        public IActionResult SearchApprenticeships(string employerName, string courseName, string providerName, string apprenticeNameOrUln, DateTime? startDate, DateTime? endDate, string selectedStatus, long? ukprn, string act)
        {
            var model = new SearchApprenticeshipsViewModel
            {
                EmployerName = employerName,
                CourseName = courseName,
                ProviderName = providerName,
                Ukprn = ukprn,
                StartDate = startDate.HasValue && startDate.Value != DateTime.MinValue ? startDate : null,
                EndDate = endDate.HasValue && endDate.Value != DateTime.MinValue ? endDate : null,
                SelectedStatus = string.IsNullOrWhiteSpace(selectedStatus) ? "" : selectedStatus,
                ApprenticeNameOrUln = apprenticeNameOrUln
            };

            switch(act)
            {
                case ActionNames.Resume:
                    ViewData.Add("FormActionRoute", RouteNames.Approval_ResumeApprenticeship);
                    ViewData.Add("FormActionText", "Resume apprenticeship(s)");
                break;
                case ActionNames.Pause:
                    ViewData.Add("FormActionRoute", RouteNames.Approval_PauseApprenticeship);
                    ViewData.Add("FormActionText", "Pause apprenticeship(s)");
                break;
                case ActionNames.Stop:
                    ViewData.Add("FormActionRoute", RouteNames.Approval_StopApprenticeship);
                    ViewData.Add("FormActionText", "Stop apprenticeship(s)");
                break;
                default:
                    return BadRequest();
            }

            return View(model);
        }

        [HttpPost("stopApprenticeship", Name = RouteNames.Approval_StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(ApprenticeshipSearchResultsViewModel model)
        {
            var tasks = new List<Task<GetApprenticeshipResult>>();
            var ids = model.SelectedIds?.Split(',');

            if (ids == null || ids.Count() == 0)
            {
                return RedirectToAction(RouteNames.Approval_SearchApprenticeships, new
                {
                    model.ApprenticeNameOrUln,
                    model.CourseName,
                    model.ProviderName,
                    model.Ukprn,
                    model.EmployerName,
                    SelectedStatus = model.Status,
                    EndDate = model.EndDate.GetValueOrDefault().ToString("yyyy-MM-dd"),
                    StartDate = model.StartDate.GetValueOrDefault().ToString("yyyy-MM-dd"),
                    act = ActionNames.Stop
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
                ApprenticeNameOrUln = model.ApprenticeNameOrUln,
                CourseName = model.CourseName,
                EmployerName = model.EmployerName,
                ProviderName = model.ProviderName,
                Ukprn = model.Ukprn,
                SelectedStatus = model.Status,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            return View(new StopApprenticeshipViewModel { Apprenticeships = _mapper.Map<List<StopApprenticeshipRow>>(results.Select(s => s.Apprenticeship)), SearchParams = searchParams });
        }

        [HttpPost("cancelStopApprenticeship", Name = RouteNames.Approval_CancelStopApprenticeship)]
        public IActionResult CancelStopApprenticeship(StopApprenticeshipViewModel model, string act)
        {
            return RedirectToAction(RouteNames.Approval_SearchApprenticeships, new
            {
                model.SearchParams.ApprenticeNameOrUln,
                model.SearchParams.CourseName,
                model.SearchParams.ProviderName,
                model.SearchParams.Ukprn,
                model.SearchParams.EmployerName,
                model.SearchParams.SelectedStatus,
                StartDate = model.SearchParams.StartDate.GetUIFormattedDate(),
                EndDate = model.SearchParams.EndDate.GetUIFormattedDate(),
                act = ActionNames.Stop
            });
        }

        [HttpPost("stopApprenticeshipConfirmation", Name = RouteNames.Approval_StopApprenticeshipConfirmation)]
        public async Task<IActionResult> StopApprenticeshipConfirmation(StopApprenticeshipViewModel model)
        {
            var claims = GetClaims();
            
            List<StopApprenticeshipRow> apprenticeshipsData;
            try
            {
                apprenticeshipsData = JsonSerializer.Deserialize<List<StopApprenticeshipRow>>(model.ApprenticeshipsData, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to deserialize apprenticeship data", e);
                ModelState.AddModelError("", "Unable to Read apprenticeship information, please return to the search and try again");
                model.ApprenticeshipsData = null;
                return View("StopApprenticeship", model);
            }

            if (string.IsNullOrWhiteSpace(claims.UserId) && string.IsNullOrWhiteSpace(claims.DisplayName))
            {
                model.Apprenticeships = apprenticeshipsData;
                ModelState.AddModelError("", "Unable to retrieve userId or name from claim for request to Stop Apprenticeship");
                return View("StopApprenticeship", model);
            }

            if (apprenticeshipsData.Any(s => s.GetStopDate == null && s.ApiSubmissionStatus != SubmissionStatus.Successful))
            {
                model.Apprenticeships = apprenticeshipsData;
                ModelState.AddModelError("", "Not all Apprenticeship rows have been supplied with a stop date.");
                return View("StopApprenticeship", model);
            }

            var stopApprenticeshipTasks = new List<Task<StopApprenticeshipResult>>();
            foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != SubmissionStatus.Successful))
            {
                stopApprenticeshipTasks.Add(_employerCommitmentsService.StopApprenticeship(new Core.Models.StopApprenticeshipRequest
                {
                    AccountId = apprenticeship.AccountId,
                    ApprenticeshipId = apprenticeship.Id,
                    StopDate = apprenticeship.GetStopDate.Value,
                    MadeRedundant = false,
                    DisplayName = claims.DisplayName,
                    EmailAddress = claims.UserEmail,
                    UserId = claims.UserId
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
                    apprenticeship.ApiSubmissionStatus = SubmissionStatus.Successful;
                    apprenticeship.ApiErrorMessage = string.Empty;
                }
                else
                {
                    apprenticeship.ApiSubmissionStatus = SubmissionStatus.Errored;
                    apprenticeship.ApiErrorMessage = result.ErrorMessage;
                }
            }

            model.Apprenticeships = apprenticeshipsData;
            return View("StopApprenticeship", model);
        }
    }
}

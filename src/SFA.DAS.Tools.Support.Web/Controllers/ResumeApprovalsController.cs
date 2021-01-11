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
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/approvals")]
    public class ResumeApprovalsController : ApprovalsControllerBase
    {
        public ResumeApprovalsController(ILogger<ResumeApprovalsController> logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper, IOptions<ClaimsConfiguration> claimConfiguration)
            : base(logger, employerCommitmentsService, mapper, claimConfiguration)
        {
        }

        [HttpPost("resumeApprenticeship", Name = RouteNames.Approval_ResumeApprenticeship)]
        public async Task<IActionResult> ResumeApprenticeship(ApprenticeshipSearchResultsViewModel model)
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
                    act = ActionNames.Resume
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
                return View(new ResumeApprenticeshipViewModel
                {
                    HasError = true
                });
            }

            return View(new ResumeApprenticeshipViewModel 
            { 
                Apprenticeships = _mapper.Map<List<ResumeApprenticeshipRow>>(results.Select(s => s.Apprenticeship)),
                SearchParams = new SearchParameters
                {
                    ApprenticeNameOrUln = model.ApprenticeNameOrUln,
                    CourseName = model.CourseName,
                    EmployerName = model.EmployerName,
                    ProviderName = model.ProviderName,
                    Ukprn = model.Ukprn,
                    SelectedStatus = model.Status,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                } 
            });
        }

        [HttpPost("cancelResumeApprenticeship", Name = RouteNames.Approval_CancelResumeApprenticeship)]
        public IActionResult CancelResumeApprenticeship(ResumeApprenticeshipViewModel model, string act)
        {
            return RedirectToAction(RouteNames.Approval_SearchApprenticeships, "Approvals", new
            {
                model.SearchParams.ApprenticeNameOrUln,
                model.SearchParams.CourseName,
                model.SearchParams.ProviderName,
                model.SearchParams.Ukprn,
                model.SearchParams.EmployerName,
                model.SearchParams.SelectedStatus,
                StartDate = model.SearchParams.StartDate.GetUIFormattedDate(),
                EndDate = model.SearchParams.EndDate.GetUIFormattedDate(),
                act = ActionNames.Resume
            });
        }

        [HttpPost("resumeApprenticeshipConfirmation", Name = RouteNames.Approval_ResumeApprenticeshipConfirmation)]
        public async Task<IActionResult> ResumeApprenticeshipConfirmation(ResumeApprenticeshipViewModel model)
        {
            var claims = GetClaims();

            if(!IsValid(model, new string[] {claims.UserId, claims.DisplayName}, out List<ResumeApprenticeshipRow> apprenticeshipsData))
            {
                return View("ResumeApprenticeship", model);
            }

            var tasks = new List<Task<ResumeApprenticeshipResult>>();
            // is this where unecessary as its captured in the validation?
            foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != SubmissionStatus.Successful))
            {
                tasks.Add(_employerCommitmentsService.ResumeApprenticeship(new ResumeApprenticeshipRequest
                {
                    ApprenticeshipId = apprenticeship.Id,
                    DisplayName = claims.DisplayName,
                    EmailAddress = claims.UserEmail,
                    UserId = claims.UserId
                }, new CancellationToken()));
            }

            var results = await Task.WhenAll(tasks);

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
            
            return View("ResumeApprenticeship", model);
        }

        public bool IsValid(ResumeApprenticeshipViewModel model, IEnumerable<string> claims, out List<ResumeApprenticeshipRow> apprenticeshipsData)
        {
            if(!model.TryDeserialise(out apprenticeshipsData, _logger))
            {
                ModelState.AddModelError(string.Empty, "Unable to Read apprenticeship information, please return to the search and try again");
                model.ApprenticeshipsData = null;

                return false;
            }
            
            if(claims.Any(c => string.IsNullOrWhiteSpace(c)))
            {
                model.Apprenticeships = apprenticeshipsData;
                ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to Resume Apprenticeship");

                return false;
            }

            // The commitments V2 API does not let you set the Resume date currently, we'll need to verify that manually adding the date is required            
            // if(apprenticeshipsData.Any(s => s.GetResumeDate == null && s.ApiSubmissionStatus != SubmissionStatus.Successful))
            // {
            //     model.Apprenticeships = apprenticeshipsData;
            //     ModelState.AddModelError(string.Empty, "Not all Apprenticeship rows have been supplied with a resume date.");

            //     return false;
            // }

            return true;
        }
    }
}

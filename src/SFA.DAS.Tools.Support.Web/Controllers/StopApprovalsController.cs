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
    public class StopApprovalsController : ApprovalsControllerBase
    {
         public StopApprovalsController(ILogger<StopApprovalsController> logger,
            IEmployerCommitmentsService employerCommitmentsService,
            IMapper mapper,
            IOptions<ClaimsConfiguration> claimConfiguration) :
            base(logger, employerCommitmentsService, mapper, claimConfiguration)
        {
        }

        [HttpPost("stopApprenticeship", Name = RouteNames.Approval_StopApprenticeship)]
        public async Task<IActionResult> StopApprenticeship(ApprenticeshipSearchResultsViewModel model)
        {
            var ids = model.SelectedIds?.Split(',');

            if (ids == null || ids.Count() == 0)
            {
                return RedirectToAction(RouteNames.Approval_SearchApprenticeships, "SearchApprovals", CreateSearchModel(model, ActionNames.Stop));
            }

            var results = await Task.WhenAll(GetApprenticeshipsFromApprovals(ids));

            if (results.Any(a => a.HasError))
            {
                var stopModelError = new StopApprenticeshipViewModel
                {
                    HasError = true
                };
                return View(stopModelError);
            }

            // Reconstruct Search Params for return to search page.
            return View(new StopApprenticeshipViewModel 
            { 
                Apprenticeships = _mapper.Map<List<StopApprenticeshipRow>>(results.Select(s => s.Apprenticeship)), 
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

        [HttpPost("cancelStopApprenticeship", Name = RouteNames.Approval_CancelStopApprenticeship)]
        public IActionResult CancelStopApprenticeship(StopApprenticeshipViewModel model, string act)
        {
            return RedirectToAction(RouteNames.Approval_SearchApprenticeships, "SearchApprovals", new
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
            
            if(!IsValid(model, new string[] {claims.UserId, claims.DisplayName}, out List<StopApprenticeshipRow> apprenticeshipsData))
            {
                return View("StopApprenticeship", model);
            }

            var tasks = new List<Task<StopApprenticeshipResult>>();
            foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != SubmissionStatus.Successful))
            {
                tasks.Add(_employerCommitmentsService.StopApprenticeship(new Core.Models.StopApprenticeshipRequest
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
            return View("StopApprenticeship", model);
        }

        public bool IsValid(StopApprenticeshipViewModel model, IEnumerable<string> claims, out List<StopApprenticeshipRow> apprenticeshipsData)
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
                ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to stop Apprenticeship");

                return false;
            }

            //The commitments V2 API does not let you set the Pause date currently, we'll need to verify that manually adding the date is required            
            if(apprenticeshipsData.Any(s => s.GetStopDate == null && s.ApiSubmissionStatus != SubmissionStatus.Successful))
            {
                model.Apprenticeships = apprenticeshipsData;
                ModelState.AddModelError(string.Empty, "Not all Apprenticeship rows have been supplied with a stop date.");

                return false;
            }

            return true;
        }
    }
}

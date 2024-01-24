using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("support/approvals")]
[Authorize(Policy = nameof(PolicyNames.HasTier3Account))]
public class ResumeApprovalsController : ApprovalsControllerBase
{
    public ResumeApprovalsController(ILogger<ResumeApprovalsController> logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper, IOptions<ClaimsConfiguration> claimConfiguration)
        : base(logger, employerCommitmentsService, mapper, claimConfiguration)
    {
    }

    [HttpPost("resumeApprenticeship", Name = RouteNames.Approval_ResumeApprenticeship)]
    public async Task<IActionResult> ResumeApprenticeship(ApprenticeshipSearchResultsViewModel model)
    {
        var ids = model.SelectedIds?.Split(',');

        if (ids == null || ids.Length == 0)
        {
            return RedirectToAction(RouteNames.Approval_SearchApprenticeships, "SearchApprovals", CreateSearchModel(model, ActionNames.Resume));
        }

        var results = await Task.WhenAll(GetApprenticeshipsFromApprovals(ids));

        if (results.Any(a => a.HasError))
        {
            return View(new ResumeApprenticeshipViewModel
            {
                HasError = true
            });
        }

        return View(new ResumeApprenticeshipViewModel
        {
            Apprenticeships = Mapper.Map<List<ResumeApprenticeshipRow>>(results.Select(s => s.Apprenticeship)),
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
    public IActionResult CancelResumeApprenticeship(ResumeApprenticeshipViewModel model)
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
            act = ActionNames.Resume
        });
    }

    [HttpPost("resumeApprenticeshipConfirmation", Name = RouteNames.Approval_ResumeApprenticeshipConfirmation)]
    public async Task<IActionResult> ResumeApprenticeshipConfirmation(ResumeApprenticeshipViewModel model)
    {
        var claims = GetClaims();

        if (!IsValid(model, new string[] { claims.UserId, claims.DisplayName }, out var apprenticeshipsData))
        {
            return View("ResumeApprenticeship", model);
        }

        var tasks = new List<Task<ResumeApprenticeshipResult>>();
        // is this where unecessary as its captured in the validation?
        foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != SubmissionStatus.Successful))
        {
            tasks.Add(EmployerCommitmentsService.ResumeApprenticeship(new ResumeApprenticeshipRequest
            {
                ApprenticeshipId = apprenticeship.Id,
                DisplayName = claims.DisplayName,
                EmailAddress = claims.UserEmail,
                UserId = claims.UserId
            }, new CancellationToken()));
        }

        var results = await Task.WhenAll(tasks);
        model.Apprenticeships = CreateApprenticeshipRows(results, apprenticeshipsData);

        return View("ResumeApprenticeship", model);
    }

    private bool IsValid(ResumeApprenticeshipViewModel model, IEnumerable<string> claims, out List<ResumeApprenticeshipRow> apprenticeshipsData)
    {
        if (!model.TryDeserialise(out apprenticeshipsData, Logger))
        {
            ModelState.AddModelError(string.Empty, "Unable to Read apprenticeship information, please return to the search and try again");
            model.ApprenticeshipsData = null;

            return false;
        }

        if (!claims.Any(string.IsNullOrWhiteSpace))
        {
            return true;
        }
        
        model.Apprenticeships = apprenticeshipsData;
        ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to Resume Apprenticeship");

        return false;
    }
}
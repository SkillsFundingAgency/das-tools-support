using AutoMapper;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("support/approvals")]
[Authorize(Policy = nameof(PolicyNames.PauseOrResumeApprenticeship))]
public class PauseApprovalsController : ApprovalsControllerBase
{
    public PauseApprovalsController(ILogger<PauseApprovalsController> logger,
        IEmployerCommitmentsService employerCommitmentsService,
        IMapper mapper,
        IOptions<ClaimsConfiguration> claimConfiguration) :
        base(logger, employerCommitmentsService, mapper, claimConfiguration)
    {
    }

    [HttpPost("pauseApprenticeship", Name = RouteNames.Approval_PauseApprenticeship)]
    public async Task<IActionResult> PauseApprenticeship(ApprenticeshipSearchResultsViewModel model)
    {
        var ids = model.SelectedIds?.Split(',');

        if (ids == null || !ids.Any())
        {
            return RedirectToAction(RouteNames.Approval_SearchApprenticeships, "SearchApprovals", CreateSearchModel(model, ActionNames.Pause));
        }

        var results = await Task.WhenAll(GetApprenticeshipsFromApprovals(ids));

        if (results.Any(a => a.HasError))
        {
            return View(new PauseApprenticeshipViewModel
            {
                HasError = true
            });
        }

        return View(new PauseApprenticeshipViewModel
        {
            Apprenticeships = Mapper.Map<List<PauseApprenticeshipRow>>(results.Select(s => s.Apprenticeship)),
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

    [HttpPost("cancelPauseApprenticeship", Name = RouteNames.Approval_CancelPauseApprenticeship)]
    public IActionResult CancelPauseApprenticeship(PauseApprenticeshipViewModel model, string act)
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
            act = ActionNames.Pause
        });
    }

    [HttpPost("pauseApprenticeshipConfirmation", Name = RouteNames.Approval_PauseApprenticeshipConfirmation)]
    public async Task<IActionResult> PauseApprenticeshipConfirmation(PauseApprenticeshipViewModel model)
    {
        var claims = GetClaims();

        if (!IsValid(model, new string[] { claims.UserId, claims.DisplayName }, out List<PauseApprenticeshipRow> apprenticeshipsData))
        {
            return View("PauseApprenticeship", model);
        }

        var tasks = new List<Task<PauseApprenticeshipResult>>();
        // is this where unecessary as its captured in the validation?
        foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != SubmissionStatus.Successful))
        {
            tasks.Add(EmployerCommitmentsService.PauseApprenticeship(new PauseApprenticeshipRequest
            {
                ApprenticeshipId = apprenticeship.Id,
                DisplayName = claims.DisplayName,
                EmailAddress = claims.UserEmail,
                UserId = claims.UserId
            }, new CancellationToken()));
        }

        var results = await Task.WhenAll(tasks);
        model.Apprenticeships = CreateApprenticeshipRows(results, apprenticeshipsData);

        return View("PauseApprenticeship", model);
    }

    private bool IsValid(PauseApprenticeshipViewModel model, IEnumerable<string> claims, out List<PauseApprenticeshipRow> apprenticeshipsData)
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
        ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to Pause Apprenticeship");

        return false;
    }
}
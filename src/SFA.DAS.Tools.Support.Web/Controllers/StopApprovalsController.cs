using AutoMapper;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

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

        if (ids == null || !ids.Any())
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
            Apprenticeships = Mapper.Map<List<StopApprenticeshipRow>>(results.Select(s => s.Apprenticeship)),
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
    public IActionResult CancelStopApprenticeship(StopApprenticeshipViewModel model)
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

        if (!IsValid(model, new string[] { claims.UserId, claims.DisplayName }, out var apprenticeshipsData))
        {
            return View("StopApprenticeship", model);
        }

        var tasks = new List<Task<StopApprenticeshipResult>>();

        foreach (var apprenticeship in apprenticeshipsData.Where(a => a.ApiSubmissionStatus != SubmissionStatus.Successful))
        {
            tasks.Add(EmployerCommitmentsService.StopApprenticeship(new Core.Models.StopApprenticeshipRequest
            {
                AccountId = apprenticeship.AccountId,
                ApprenticeshipId = apprenticeship.Id,
                CurrentStopDate = apprenticeship.Status == "Stopped" ? apprenticeship.StatusDate : null,
                RequestedStopDate = apprenticeship.GetStopDate.Value,
                MadeRedundant = false,
                DisplayName = claims.DisplayName,
                EmailAddress = claims.UserEmail,
                UserId = claims.UserId
            }, new CancellationToken()));
        }

        var results = await Task.WhenAll(tasks);

        SetUpdatedStopDate(results, apprenticeshipsData);

        model.Apprenticeships = CreateApprenticeshipRows(results, apprenticeshipsData);

        return View("StopApprenticeship", model);
    }

    private static void SetUpdatedStopDate(IEnumerable<StopApprenticeshipResult> results, IReadOnlyCollection<StopApprenticeshipRow> apprenticeshipsData)
    {
        foreach (var resultApprenticeship in results)
        {
            if (resultApprenticeship.HasError) continue;

            var apprenticeship = apprenticeshipsData.SingleOrDefault(app => app.Id == resultApprenticeship.ApprenticeshipId);

            if (apprenticeship == null) continue;
            apprenticeship.StatusDate = resultApprenticeship.StopDate;
        }
    }

    private bool IsValid(StopApprenticeshipViewModel model, IEnumerable<string> claims, out List<StopApprenticeshipRow> apprenticeshipsData)
    {
        if (!model.TryDeserialise(out apprenticeshipsData, Logger))
        {
            ModelState.AddModelError(string.Empty, "Unable to Read apprenticeship information, please return to the search and try again");
            model.ApprenticeshipsData = null;

            return false;
        }

        if (claims.Any(string.IsNullOrWhiteSpace))
        {
            model.Apprenticeships = apprenticeshipsData;
            ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to stop apprenticeship");

            return false;
        }

        if (!apprenticeshipsData.Any(s => s.GetStopDate == null && s.ApiSubmissionStatus != SubmissionStatus.Successful))
        {
            return true;
        }

        model.Apprenticeships = apprenticeshipsData;
        ModelState.AddModelError(string.Empty, "Not all Apprenticeship rows have been supplied with a stop date.");

        return false;
    }
}
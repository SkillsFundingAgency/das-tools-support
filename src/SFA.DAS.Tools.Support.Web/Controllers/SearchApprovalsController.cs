using AutoMapper;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("support/approvals")]
public class SearchApprovalsController : ApprovalsControllerBase
{
    private readonly IAuthorizationService _authorizationService;
    public SearchApprovalsController(ILogger<SearchApprovalsController> logger,
        IEmployerCommitmentsService employerCommitmentsService,
        IMapper mapper,
        IOptions<ClaimsConfiguration> claimConfiguration,
        IAuthorizationService authorizationService) :
        base(logger, employerCommitmentsService, mapper, claimConfiguration)
    {
        _authorizationService = authorizationService;
    }

    [HttpGet("searchApprenticeships", Name = RouteNames.Approval_SearchApprenticeships)]
    public async Task<IActionResult> SearchApprenticeships(ApprovalSearchApprenticeshipRequest request)
    {
        var hasTier3Account = await _authorizationService.AuthorizeAsync(User, nameof(PolicyNames.Privileged));
        if (!hasTier3Account.Succeeded && request.Act is ActionNames.Resume or ActionNames.Pause)
        {
            return Forbid();
        }

        var model = new SearchApprenticeshipsViewModel
        {
            EmployerName = request.EmployerName,
            CourseName = request.CourseName,
            ProviderName = request.ProviderName,
            Ukprn = request.Ukprn,
            StartDate = request.StartDate.HasValue && request.StartDate.Value != DateTime.MinValue ? request.StartDate : null,
            EndDate = request.EndDate.HasValue && request.EndDate.Value != DateTime.MinValue ? request.EndDate : null,
            SelectedStatus = string.IsNullOrWhiteSpace(request.SelectedStatus) ? "" : request.SelectedStatus,
            ApprenticeNameOrUln = request.ApprenticeNameOrUln
        };

        switch (request.Act)
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
}
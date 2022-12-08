using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/approvals")]
    [Authorize(Policy = nameof(PolicyNames.HasTier2Tier3Account))]
    public class SearchApprovalsController : ApprovalsControllerBase
    {
        private IAuthorizationService _authorizationService;
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
        public IActionResult SearchApprenticeships(string employerName, string courseName, string providerName, string apprenticeNameOrUln, DateTime? startDate, DateTime? endDate, string selectedStatus, long? ukprn, string act)
        {
            if (_authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier2Account)).Result.Succeeded && act != ActionNames.Stop) return Forbid();

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

            switch (act)
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
}

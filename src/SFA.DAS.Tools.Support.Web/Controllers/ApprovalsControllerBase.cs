using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public abstract class ApprovalsControllerBase : Controller
    {
        protected readonly ILogger _logger;
        protected readonly IEmployerCommitmentsService _employerCommitmentsService;
        protected readonly IMapper _mapper;
        protected readonly IOptions<ClaimsConfiguration> _claimConfiguration;

        public ApprovalsControllerBase(ILogger logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper, IOptions<ClaimsConfiguration> claimConfiguration)
        {
            _logger = logger;
            _employerCommitmentsService = employerCommitmentsService;
            _mapper = mapper;
            _claimConfiguration = claimConfiguration;
            _claimConfiguration.Value.ValidateConfiguration();
        }

        protected dynamic GetClaims()
        {
            return new 
            {
                UserEmail = HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.EmailClaim),
                UserId = HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.EmailClaim),
                DisplayName = $"{HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.NameClaim)} {HttpContext.User.Claims.GetClaim(_claimConfiguration.Value.NameIdentifierClaim)}"
            };
        }

        protected dynamic CreateSearchModel(ApprenticeshipSearchResultsViewModel model, string action)
        {
            return new
            {
                model.ApprenticeNameOrUln,
                model.CourseName,
                model.ProviderName,
                model.Ukprn,
                model.EmployerName,
                SelectedStatus = model.Status,
                EndDate = model.EndDate.GetUIFormattedDate(),
                StartDate = model.StartDate.GetUIFormattedDate(),
                act = action
            };
        }

        protected List<Task<GetApprenticeshipResult>> GetApprenticeshipsFromApprovals(string[] ids)
        {
            var tasks = new List<Task<GetApprenticeshipResult>>();

            foreach (var id in ids)
            {
                if (int.TryParse(id, out var longId))
                {
                    tasks.Add(_employerCommitmentsService.GetApprenticeship(longId, new CancellationToken()));
                }
            }

            return tasks;
        }
    }
}
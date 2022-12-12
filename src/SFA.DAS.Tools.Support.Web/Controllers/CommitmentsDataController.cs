using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public class CommitmentsDataController : Controller
    {
        private readonly ILogger<CommitmentsDataController> _logger;
        private readonly IEmployerCommitmentsService _employerCommitmentsService;

        public CommitmentsDataController(ILogger<CommitmentsDataController> logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper)
        {
            _logger = logger;
            _employerCommitmentsService = employerCommitmentsService;
        }

        [HttpPost("[controller]/search", Name = RouteNames.CommitmentsData_SearchApprenticeships)]
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
                    Ukprn = model.Ukprn,
                    SearchTerm = model.ApprenticeNameOrUln,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ApprenticeshipStatus = model.SelectedStatus
                }, new CancellationToken());

            if (result.HasError)
            {
                _logger.LogError($"Call to Commitments Api Failed with error: {result.ErrorMessage}");
                return Json(new { ErrorTitle = "Call to Commitments Api Failed", ErrorMessage = result.ErrorMessage });
            }

            return Json(result.Apprenticeships.Select(a => new StopApprenticeshipRow
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                EmployerName = a.EmployerName,
                ProviderName = a.ProviderName,
                Ukprn = a.Ukprn,
                CourseName = a.CourseName,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Uln = a.Uln,
                Status = a.ApprenticeshipStatus.ToString(),
                PaymentStatus = a.PaymentStatus.ToString(),
                CohortReference = a.CohortReference,
            }));
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/commitmentsdata")]
    public class CommitmentsDataController : Controller
    {
        private readonly ILogger<CommitmentsDataController> _logger;
        private readonly IEmployerCommitmentsService _employerCommitmentsService;
        private readonly IMapper _mapper;

        public CommitmentsDataController(ILogger<CommitmentsDataController> logger, IEmployerCommitmentsService employerCommitmentsService, IMapper mapper)
        {
            _logger = logger;
            _employerCommitmentsService = employerCommitmentsService;
            _mapper = mapper;
        }

        [HttpPost("search", Name = RouteNames.CommitmentsData_SearchApprenticeships)]
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
                    SearchTerm = model.ApprenticeName,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    ApprenticeshipStatus = model.SelectedStatus
                }, new CancellationToken());

            if (result.HasError)
            {
                return Json(new { ErrorTitle = "Call to Commitments Api Failed", ErrorMessage = result.ErrorMessage });
            }

            return Json(result.Apprenticeships.Select(a => new StopApprenticeshipRow
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                EmployerName = a.EmployerName,
                ProviderName = a.ProviderName,
                CourseName = a.CourseName,
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Status = a.ApprenticeshipStatus.ToString(),
                PaymentStatus = a.PaymentStatus.ToString(),
            }));
        }
    }
}

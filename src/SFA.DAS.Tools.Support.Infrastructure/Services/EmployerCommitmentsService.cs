using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using System.Collections.Generic;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerCommitmentsService
    {
        Task<StopApprenticeshipResult> StopApprenticeship(long employerAccountId, long apprenticeshipId, string UserId, DateTime stopDate);
        Task<ApprenticeshipSummaryResult> GetApprenticeshipSummary(long apprenticeshipId, long employerAccountId);
        Task<SearchApprenticeshipsResult> SearchApprenticeships(string courseName, string employerName, string providerName, string searchTerm, DateTime? startDate, DateTime? endDate);
        Task<GetApprenticeshipResult> GetApprenticeship(long id);
    }

    public class EmployerCommitmentsService : IEmployerCommitmentsService
    {
        private readonly ICommitmentsApiClient _commitmentApi;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EmployerCommitmentsService(ICommitmentsApiClient commitmentApi, IMapper mapper, ILogger<EmployerCommitmentsService> logger)
        {
            _commitmentApi = commitmentApi;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<StopApprenticeshipResult> StopApprenticeship(long employerAccountId, long apprenticeshipId, string userId, DateTime stopDate)
        {
            try
            {
                if (employerAccountId <= 0)
                {
                    throw new ArgumentException("employerAccountId must be greater than 0", "employerAccountId");
                }

                if (apprenticeshipId <= 0)
                {
                    throw new ArgumentException("apprenticeshipId must be greater than 0", "apprenticeshipId");
                }

                // Stop Apprenticeship to be subbed in .
                //await _commitmentApi.Stop(employerAccountId, apprenticeshipId, new ApprenticeshipSubmission
                //{
                //    DateOfChange = stopDate,
                //    PaymentStatus = Withdrawn,
                //    UserId = userId
                //});
                await Task.FromResult(0);
                return new StopApprenticeshipResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to stop the apprenticeship.");
                return new StopApprenticeshipResult
                {
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<ApprenticeshipSummaryResult> GetApprenticeshipSummary(long apprenticeshipId, long employerAccountId)
        {
            try
            {
                if (employerAccountId <= 0)
                {
                    throw new ArgumentException("employerAccountId must be greater than 0", "employerAccountId");
                }

                if (apprenticeshipId <= 0)
                {
                    throw new ArgumentException("apprenticeshipId must be greater than 0", "apprenticeshipId");
                }

                // switch for get employer apprenticeship
                //var result = await _commitmentApi.GetEmployerApprenticeship(employerAccountId, apprenticeshipId);

                //return _mapper.Map<ApprenticeshipSummaryResult>(result);
                await Task.FromResult(0);
                return new ApprenticeshipSummaryResult();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to fetch the apprenticeship.");
                return new ApprenticeshipSummaryResult
                {
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<SearchApprenticeshipsResult> SearchApprenticeships(string courseName, string employerName, string providerName, string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var request = new GetApprenticeshipsRequest
                {
                    CourseName = courseName,
                    EmployerName = employerName,
                    ProviderName = providerName,
                    SearchTerm = searchTerm,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var result = await _commitmentApi.GetApprenticeships(request);

                return new SearchApprenticeshipsResult
                {
                    Apprenticeships = _mapper.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, List<ApprenticeshipDto>>(result.Apprenticeships),
                    ResultCount = result.TotalApprenticeshipsFound
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to search for apprenticeships.");
                return new SearchApprenticeshipsResult
                {
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<GetApprenticeshipResult> GetApprenticeship(long apprenticeshipId)
        {
            try
            {
                var result = await _commitmentApi.GetApprenticeship(apprenticeshipId);

                return new GetApprenticeshipResult
                {
                    Apprenticeship = _mapper.Map<ApprenticeshipDto>(result),
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve apprenticeship.");
                return new GetApprenticeshipResult
                {
                    ErrorMessage = e.Message
                };
            }
        }
    }
}

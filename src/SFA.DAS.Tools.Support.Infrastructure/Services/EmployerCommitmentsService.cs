using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using AutoMapper;
using SFA.DAS.CommitmentsV2.Api.Client;
using System.Collections.Generic;
using System.Threading;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using System.Linq;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerCommitmentsService
    {
        Task<StopApprenticeshipResult> StopApprenticeship(Core.Models.StopApprenticeshipRequest request, CancellationToken token);
        Task<SearchApprenticeshipsResult> SearchApprenticeships(string courseName, string employerName, string providerName, string searchTerm, DateTime? startDate, DateTime? endDate, CancellationToken token);
        Task<GetApprenticeshipResult> GetApprenticeship(long id, CancellationToken token);
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

        public async Task<StopApprenticeshipResult> StopApprenticeship(Core.Models.StopApprenticeshipRequest request, CancellationToken token)
        {
            try
            {
                request.Validate();

                await _commitmentApi.StopApprenticeship(request.ApprenticeshipId, new CommitmentsV2.Api.Types.Requests.StopApprenticeshipRequest
                {
                    AccountId = request.AccountId,
                    MadeRedundant = request.MadeRedundant,
                    StopDate = request.StopDate,
                    UserInfo = new CommitmentsV2.Types.UserInfo
                    {
                        UserId = request.UserId,
                        UserDisplayName = request.DisplayName,
                        UserEmail = request.EmailAddress
                    }
                }, token);
                
                return new StopApprenticeshipResult();
            }
            catch(CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to stop the apprenticeship.");
                var errorMessages = string.Empty;
                return new StopApprenticeshipResult
                {
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a,b) => a + " " + b.Message)
                };
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

        public async Task<SearchApprenticeshipsResult> SearchApprenticeships(string courseName, string employerName, string providerName, string searchTerm, DateTime? startDate, DateTime? endDate, CancellationToken token)
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

                var result = await _commitmentApi.GetApprenticeships(request, token);

                return new SearchApprenticeshipsResult
                {
                    Apprenticeships = _mapper.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, List<ApprenticeshipDto>>(result.Apprenticeships),
                    ResultCount = result.TotalApprenticeshipsFound
                };
            }
            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to stop the apprenticeship.");
                var errorMessages = string.Empty;
                return new SearchApprenticeshipsResult
                {
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => a + " " + b.Message)
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

        public async Task<GetApprenticeshipResult> GetApprenticeship(long apprenticeshipId, CancellationToken token)
        {
            try
            {
                var result = await _commitmentApi.GetApprenticeship(apprenticeshipId, token);

                return new GetApprenticeshipResult
                {
                    Apprenticeship = _mapper.Map<ApprenticeshipDto>(result),
                };
            }
            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to stop the apprenticeship.");
                var errorMessages = string.Empty;
                return new GetApprenticeshipResult
                {
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => a + " " + b.Message)
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

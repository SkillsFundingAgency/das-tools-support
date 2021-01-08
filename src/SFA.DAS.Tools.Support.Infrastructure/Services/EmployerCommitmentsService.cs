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
using SFA.DAS.CommitmentsV2.Types;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerCommitmentsService
    {
        Task<StopApprenticeshipResult> StopApprenticeship(Core.Models.StopApprenticeshipRequest request, CancellationToken token);
        Task<PauseApprenticeshipResult> PauseApprenticeship(Core.Models.PauseApprenticeshipRequest request, CancellationToken token);
        Task<SearchApprenticeshipsResult> SearchApprenticeships(SearchApprenticeshipsRequest request, CancellationToken token);
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

                return new StopApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId
                };
            }
            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to stop the apprenticeship.");
                var errorMessages = string.Empty;
                return new StopApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => a + " " + b.Message)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to stop the apprenticeship.");
                return new StopApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<SearchApprenticeshipsResult> SearchApprenticeships(SearchApprenticeshipsRequest request, CancellationToken token)
        {
            try
            {
                CommitmentsV2.Types.ApprenticeshipStatus? status = null;
                if(int.TryParse(request.ApprenticeshipStatus, out var statusInt))
                {
                    status = (CommitmentsV2.Types.ApprenticeshipStatus)statusInt;
                }

                var result = await _commitmentApi.GetApprenticeships(new GetApprenticeshipsRequest
                {
                    CourseName = request.CourseName,
                    EmployerName = request.EmployerName,
                    ProviderName = request.ProviderName,
                    ProviderId = request.Ukprn,
                    SearchTerm = request.SearchTerm,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Status = status
                }, token);

                return new SearchApprenticeshipsResult
                {
                    Apprenticeships = _mapper.Map<IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, List<ApprenticeshipDto>>(result.Apprenticeships),
                    ResultCount = result.TotalApprenticeshipsFound
                };
            }
            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to search for apprenticeships.");
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
                if (apprenticeshipId <= 0)
                {
                    throw new ArgumentException("ApprenticeshipId must be greater than 0", "apprenticeshipId");
                }

                var result = await _commitmentApi.GetApprenticeship(apprenticeshipId, token);

                return new GetApprenticeshipResult
                {
                    Apprenticeship = _mapper.Map<ApprenticeshipDto>(result)
                };
            }
            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to retrieve apprenticeship.");
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

        public async Task<PauseApprenticeshipResult> PauseApprenticeship(Core.Models.PauseApprenticeshipRequest request, CancellationToken token)
        {
            try
            {
                request.Validate();

                await _commitmentApi.PauseApprenticeship(new CommitmentsV2.Api.Types.Requests.PauseApprenticeshipRequest
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    UserInfo = new CommitmentsV2.Types.UserInfo
                    {
                        UserId = request.UserId,
                        UserDisplayName = request.DisplayName,
                        UserEmail = request.EmailAddress
                    }
                }, token);

                return new PauseApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId
                };
            }

            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to stop the apprenticeship.");
                var errorMessages = string.Empty;
                return new PauseApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => a + " " + b.Message)
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to stop the apprenticeship.");
                return new PauseApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}

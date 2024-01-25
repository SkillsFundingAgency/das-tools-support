using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Api.Types.Validation;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Tools.Support.Core.Models;
using ApprenticeshipStatus = SFA.DAS.CommitmentsV2.Types.ApprenticeshipStatus;
using PauseApprenticeshipRequest = SFA.DAS.Tools.Support.Core.Models.PauseApprenticeshipRequest;
using ResumeApprenticeshipRequest = SFA.DAS.Tools.Support.Core.Models.ResumeApprenticeshipRequest;
using StopApprenticeshipRequest = SFA.DAS.Tools.Support.Core.Models.StopApprenticeshipRequest;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
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

        public async Task<StopApprenticeshipResult> StopApprenticeship(StopApprenticeshipRequest request, CancellationToken token)
        {
            try
            {
                request.Validate();

                if (request.CurrentStopDate == null)
                {
                    await _commitmentApi.StopApprenticeship(request.ApprenticeshipId, new CommitmentsV2.Api.Types.Requests.StopApprenticeshipRequest
                    {
                        AccountId = request.AccountId,
                        MadeRedundant = request.MadeRedundant,
                        StopDate = request.RequestedStopDate,
                        UserInfo = new UserInfo
                        {
                            UserId = request.UserId,
                            UserDisplayName = request.DisplayName,
                            UserEmail = request.EmailAddress
                        }
                    }, token);
                }
                else
                {
                    await _commitmentApi.UpdateApprenticeshipStopDate(request.ApprenticeshipId, new ApprenticeshipStopDateRequest
                    {
                        AccountId = request.AccountId,
                        NewStopDate = request.RequestedStopDate,
                        UserInfo = new UserInfo
                        {
                            UserId = request.UserId,
                            UserDisplayName = request.DisplayName,
                            UserEmail = request.EmailAddress
                        }
                    }, token);
                }

                return new StopApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    StopDate = request.RequestedStopDate
                };
            }
            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to stop the apprenticeship.");
                var errorMessages = string.Empty;
                return new StopApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => $"{a} {b.Message}")
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
                ApprenticeshipStatus? status = null;
                if (int.TryParse(request.ApprenticeshipStatus, out var statusInt))
                {
                    status = (ApprenticeshipStatus)statusInt;
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
                    Status = status,
                    PageNumber = 1,
                    PageItemCount = 99_999 //HACK: The CommitmentsV2 API has a "DownLoad" limit applied to it when you ask for page 0 that returns only apprenticeships with an EndDate within the last 12 months. This hack circumvents having that limit applied
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
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => $"{a} {b.Message}")
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
                    throw new Exception("ApprenticeshipId must be greater than 0");
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
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => $"{a} {b.Message}")
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

        public async Task<PauseApprenticeshipResult> PauseApprenticeship(PauseApprenticeshipRequest request, CancellationToken token)
        {
            try
            {
                request.Validate();

                await _commitmentApi.PauseApprenticeship(new CommitmentsV2.Api.Types.Requests.PauseApprenticeshipRequest
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    UserInfo = new UserInfo
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
                _logger.LogError(cException, "Failure to pause the apprenticeship.");
                var errorMessages = string.Empty;
                return new PauseApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => $"{a} {b.Message}")
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to pause the apprenticeship.");
                return new PauseApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = e.Message
                };
            }
        }

        public async Task<ResumeApprenticeshipResult> ResumeApprenticeship(ResumeApprenticeshipRequest request, CancellationToken token)
        {
            try
            {
                request.Validate();

                await _commitmentApi.ResumeApprenticeship(new CommitmentsV2.Api.Types.Requests.ResumeApprenticeshipRequest
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    UserInfo = new UserInfo
                    {
                        UserId = request.UserId,
                        UserDisplayName = request.DisplayName,
                        UserEmail = request.EmailAddress
                    }
                }, token);

                return new ResumeApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId
                };
            }

            catch (CommitmentsApiModelException cException)
            {
                _logger.LogError(cException, "Failure to resume the apprenticeship.");
                var errorMessages = string.Empty;
                return new ResumeApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = cException.Errors.Aggregate(errorMessages, (a, b) => $"{a} {b.Message}")
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to resume the apprenticeship.");
                return new ResumeApprenticeshipResult
                {
                    ApprenticeshipId = request.ApprenticeshipId,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}
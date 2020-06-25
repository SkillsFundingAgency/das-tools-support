using System;
using SFA.DAS.Commitments.Api.Client;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;
using AutoMapper;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerCommitmentsService
    {
        Task<StopApprenticeshipResult> StopApprenticeship(long employerAccountId, long apprenticeshipId, string UserId, DateTime stopDate);
        Task<ApprenticeshipSummaryResult> GetApprenticeshipSummary(long apprenticeshipId, long employerAccountId);
    }

    public class EmployerCommitmentsService : IEmployerCommitmentsService
    {
        private readonly IEmployerCommitmentApi _employerCommitmentApi;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public EmployerCommitmentsService(IEmployerCommitmentApi employerCommitmentApi, IMapper mapper, ILogger<EmployerCommitmentsService> logger)
        {
            _employerCommitmentApi = employerCommitmentApi;
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

                if(apprenticeshipId <= 0)
                {
                    throw new ArgumentException("apprenticeshipId must be greater than 0", "apprenticeshipId");
                }

                await _employerCommitmentApi.PatchEmployerApprenticeship(employerAccountId, apprenticeshipId, new ApprenticeshipSubmission
                {
                    DateOfChange = stopDate,
                    PaymentStatus = Commitments.Api.Types.Apprenticeship.Types.PaymentStatus.Withdrawn,
                    UserId = userId
                });

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

                var result = await _employerCommitmentApi.GetEmployerApprenticeship(employerAccountId, apprenticeshipId);

                return _mapper.Map<ApprenticeshipSummaryResult>(result);
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
    }
}

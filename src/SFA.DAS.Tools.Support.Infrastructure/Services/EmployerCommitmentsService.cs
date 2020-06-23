using System;
using SFA.DAS.Commitments.Api.Client;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Infrastructure.Services
{
    public interface IEmployerCommitmentsService
    {
        Task<StopApprenticeshipResult> StopApprenticeship(long employerAccountId, long apprenticeshipId, string UserId, DateTime stopDate);
    }

    public class EmployerCommitmentsService : IEmployerCommitmentsService
    {
        private readonly IEmployerCommitmentApi _employerCommitmentApi;
        private readonly ILogger _logger;

        public EmployerCommitmentsService(IEmployerCommitmentApi employerCommitmentApi, ILogger<EmployerCommitmentsService> logger)
        {
            _employerCommitmentApi = employerCommitmentApi;
            _logger = logger;
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
    }
}

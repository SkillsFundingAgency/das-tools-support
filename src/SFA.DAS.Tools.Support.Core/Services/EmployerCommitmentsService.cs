using System;
using SFA.DAS.Commitments.Api.Client;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Core.Models;

namespace SFA.DAS.Tools.Support.Core.Services
{
    public interface IEmployerCommitmentsService
    {
        Task<StopApprenticeshipResult> StopApprenticeship(long employerAccountId, long apprenticeshipId, DateTime stopDate);
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

        public async Task<StopApprenticeshipResult> StopApprenticeship(long employerAccountId, long apprenticeshipId, DateTime stopDate)
        {
            try
            {
                await _employerCommitmentApi.PatchEmployerApprenticeship(employerAccountId, apprenticeshipId, new ApprenticeshipSubmission
                {
                    DateOfChange = stopDate,
                    PaymentStatus = Commitments.Api.Types.Apprenticeship.Types.PaymentStatus.Withdrawn
                });

                return default;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failure to call Commitments Api");
                return new StopApprenticeshipResult
                {
                    ErrorMessage = e.Message
                };
            }
        }
    }
}

using System;
using SFA.DAS.Commitments.Api.Client;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.Tools.Support.Core.Services
{
    public interface IEmployerCommitmentsService
    {
        Task StopApprenticeship(long employerAccountId, long apprenticeshipId, DateTime stopDate);
    }

    public class EmployerCommitmentsService : IEmployerCommitmentsService
    {
        private readonly IEmployerCommitmentApi _employerCommitmentApi;

        //        public EmployerCommitmentsService(IEmployerCommitmentApi employerCommitmentApi)
        public EmployerCommitmentsService()
        {
            //_employerCommitmentApi = employerCommitmentApi;
        }

        public async Task StopApprenticeship(long employerAccountId, long apprenticeshipId, DateTime stopDate)
        {
            await _employerCommitmentApi.PatchEmployerApprenticeship(employerAccountId, apprenticeshipId, new ApprenticeshipSubmission
            {
                DateOfChange = stopDate,
                PaymentStatus = Commitments.Api.Types.Apprenticeship.Types.PaymentStatus.Withdrawn
            });
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;

public class GetApprenticeshipDetailsQueryHandler(IToolsSupportApimService employerSupportApiClient, IEncodingService encodingService) : IRequestHandler<GetApprenticeshipDetailsQuery, GetApprenticeshipDetailsQueryResult>
{
    public async Task<GetApprenticeshipDetailsQueryResult> Handle(GetApprenticeshipDetailsQuery query, CancellationToken cancellationToken)
    {
        var id = encodingService.Decode(query.HashedApprenticeshipId, EncodingType.ApprenticeshipId);

        var response = await employerSupportApiClient.GetApprenticeshipDetails(id, cancellationToken);

        if (response == null)
            return null;

        if (encodingService.Encode(response.EmployerAccountId, EncodingType.AccountId) != query.HashedAccountId)
            throw new UnauthorizedAccessException("This Employer Account does not have access to this apprenticeship");

        var result = new GetApprenticeshipDetailsQueryResult
        {
            HashedApprenticeshipId = query.HashedApprenticeshipId,
            PaymentStatus = response.PaymentStatus,
            AgreementStatus = response.AgreementStatus,
            ConfirmationStatusDescription = response.ConfirmationStatusDescription,
            FirstName = response.FirstName,
            LastName = response.LastName,
            Email = response.Email,
            Uln = response.Uln,
            DateOfBirth = response.DateOfBirth,
            CohortReference = response.CohortReference,
            EmployerReference = response.EmployerReference,
            LegalEntity = response.LegalEntity,
            TrainingProvider = response.TrainingProvider,
            UKPRN = response.UKPRN,
            Trainingcourse = response.Trainingcourse,
            ApprenticeshipCode = response.ApprenticeshipCode,
            TrainingStartDate = response.TrainingStartDate,
            TrainingEndDate = response.TrainingEndDate,
            TrainingCost = response.TrainingCost,
            Version = response.Version,
            Option = response.Option,
            PauseDate = response.PauseDate,
            StopDate = response.StopDate,
            CompletionDate = response.CompletionDate,
            MadeRedundant = response.MadeRedundant,
            OverlappingTrainingDateRequestCreatedOn = response.OverlappingTrainingDateRequestCreatedOn,
            PendingChanges = response.PendingChanges,
            ChangeOfProviderChain = response.ChangeOfProviderChain
       };

        return result;
    }
}
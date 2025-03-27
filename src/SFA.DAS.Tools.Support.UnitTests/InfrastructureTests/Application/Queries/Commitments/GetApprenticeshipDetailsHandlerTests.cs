using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.Commitments;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.Commitments;

public class GetApprenticeshipDetailsHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetApprenticeshipDetailsQueryResult_WhenCalled(
        long apprenticeshipId,
        GetApprenticeshipDetailsQuery query,
        GetApprenticeshipDetailsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetApprenticeshipDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.AccountId)).Returns(query.HashedAccountId);
        encodingService.Setup(x => x.Decode(query.HashedApprenticeshipId, EncodingType.ApprenticeshipId)).Returns(apprenticeshipId);

        employerSupportApiClient.Setup(o => o.GetApprenticeshipDetails(apprenticeshipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(new GetApprenticeshipDetailsQueryResult
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
        });
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnNullWhenNotFound(
        long apprenticeshipId,
        GetApprenticeshipDetailsQuery query,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetApprenticeshipDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Decode(query.HashedApprenticeshipId, EncodingType.ApprenticeshipId)).Returns(apprenticeshipId);

        employerSupportApiClient.Setup(o => o.GetApprenticeshipDetails(apprenticeshipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetApprenticeshipDetailsResponse)null);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldThrowAccessDeniedExceptionWhenApprenticeshipNotBelongingToEmployerAccount(
        long apprenticeshipId,
        GetApprenticeshipDetailsQuery query,
        GetApprenticeshipDetailsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetApprenticeshipDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.AccountId)).Returns(query.HashedAccountId + "XXX");
        encodingService.Setup(x => x.Decode(query.HashedApprenticeshipId, EncodingType.ApprenticeshipId)).Returns(apprenticeshipId);

        employerSupportApiClient.Setup(o => o.GetApprenticeshipDetails(apprenticeshipId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        Func<Task> accessDenied = async () => await handler.Handle(query, CancellationToken.None);

        accessDenied.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
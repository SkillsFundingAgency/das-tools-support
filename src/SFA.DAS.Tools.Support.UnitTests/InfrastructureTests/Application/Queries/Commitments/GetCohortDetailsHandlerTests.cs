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

public class GetCohortDetailsHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetCohortQueryResult_WhenCalled(
        GetCohortDetailsQuery query,
        GetCohortResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetCohortDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.AccountId)).Returns("XXXXX");

        employerSupportApiClient.Setup(o => o.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(new GetCohortDetailsQueryResult
        {
            CohortId = response.CohortId,
            CohortReference = response.CohortReference,
            HashedAccountId = "XXXXX",
            EmployerAccountName = response.EmployerAccountName,
            ProviderName = response.ProviderName,
            UkPrn = response.UkPrn,
            CohortStatus = response.CohortStatus,
            Apprenticeships = response.ApprovedApprenticeships
        });
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldCallApiClientWithCorrectParameters(
        long cohortId,
        GetCohortDetailsQuery query,
        GetCohortResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetCohortDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Decode(query.CohortRef.ToUpper(), EncodingType.CohortReference)).Returns(cohortId);

        employerSupportApiClient.Setup(o => o.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        employerSupportApiClient.Verify(client => client.GetCohort(It.IsAny<long>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnNullWhenNotFound(
        long cohortId,
        GetCohortDetailsQuery query,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetCohortDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Decode(query.CohortRef.ToUpper(), EncodingType.CohortReference)).Returns(cohortId);

        employerSupportApiClient.Setup(o => o.GetCohort(cohortId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetCohortResponse)null);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }
}

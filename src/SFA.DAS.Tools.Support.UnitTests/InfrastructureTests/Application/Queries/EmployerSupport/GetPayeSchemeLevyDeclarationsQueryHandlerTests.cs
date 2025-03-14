using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.EmployerSupport;
public class GetPayeSchemeLevyDeclarationsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturn_Model_WhenqueryIsValid(
        long accountId,
        string actualPayeRef,
        GetPayeSchemeLevyDeclarationsQuery query,
        GetPayeSchemeLevyDeclarationsResponse response,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IPayeRefHashingService> refHashingService,
        GetPayeSchemeLevyDeclarationsQueryHandler handler
    )
    {
        // Arrange
        encodingService.Setup(x => x.Decode(query.HashedAccountId, EncodingType.AccountId))
            .Returns(accountId);

        refHashingService.Setup(x => x.DecodeValueToString(query.HashedPayeRef))
            .Returns(actualPayeRef);

        mockApiClient.Setup(client => client.GetPayeSchemeLevyDeclarations(accountId, actualPayeRef, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        mockApiClient.Verify();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(response);
    }
}

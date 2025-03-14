using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChallengeEntry;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Commands.EmployerSupport;
public class ChallengeEntryCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturn_Model_WhenCommandIsValid(
        long accountId,
        ChallengeEntryCommand command,
        ChallengeEntryResponse response,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        ChallengeEntryCommandHandler handler
      )
    {
        // Arrange
        encodingService.Setup(x => x.Decode(command.HashedAccountId, EncodingType.AccountId))
            .Returns(accountId);

        mockApiClient.Setup(client => client.ChallengeEntry(It.Is<ChallengeEntryRequest>(req =>
            req.Id == command.HashedAccountId &&
            req.Challenge1 == command.Challenge1 &&
            req.Challenge2 == command.Challenge2 &&
            req.Balance == command.Balance &&
            req.FirstCharacterPosition == command.FirstCharacterPosition &&
            req.SecondCharacterPosition == command.SecondCharacterPosition
            ), accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockApiClient.Verify();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(response);
    }
}

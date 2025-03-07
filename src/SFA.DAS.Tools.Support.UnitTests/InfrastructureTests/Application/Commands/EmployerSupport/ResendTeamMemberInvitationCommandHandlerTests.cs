using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Commands.EmployerSupport;

public class ResendTeamMemberInvitationCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid(
        ResendTeamMemberInvitationCommand command,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<ILogger<ResendTeamMemberInvitationCommandHandler>> mockLogger,
        ResendTeamMemberInvitationCommandHandler handler
        )
    {
        // Arrange
        mockApiClient.Setup(client => client.ResendInvitation(It.Is<ResendInvitationRequest>(req =>
            req.HashedAccountId == command.HashedAccountId &&
            req.Email == command.Email), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockApiClient.Verify();
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnFailure_WhenExceptionIsThrown(
        ResendTeamMemberInvitationCommand command,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<ILogger<ResendTeamMemberInvitationCommandHandler>> mockLogger,
        ResendTeamMemberInvitationCommandHandler handler
        )
    {
        // Arrange
        mockApiClient.Setup(client => client.ResendInvitation(It.IsAny<ResendInvitationRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some error"))
            .Verifiable();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        mockApiClient.Verify();
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
    }
}

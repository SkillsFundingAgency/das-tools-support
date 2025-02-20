using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Commands.EmployerSupport;

public class SendTeamMemberInviteCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid(
        SendTeamMemberInviteCommand command,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<ILogger<SendTeamMemberInviteCommandHandler>> mockLogger,
        SendTeamMemberInviteCommandHandler handler
        )
    {
        // Arrange
        mockApiClient.Setup(client => client.SendInvitation(It.Is<SendInvitationRequest>(req =>
            req.HashedAccountId == command.HashedAccountId &&
            req.FullName == command.FullName &&
            req.Email == command.Email &&
            req.Role == command.Role), It.IsAny<CancellationToken>()))
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
        SendTeamMemberInviteCommand command,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<ILogger<SendTeamMemberInviteCommandHandler>> mockLogger,
        SendTeamMemberInviteCommandHandler handler
        )
    {
        // Arrange
        mockApiClient.Setup(client => client.SendInvitation(It.IsAny<SendInvitationRequest>(), It.IsAny<CancellationToken>()))
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

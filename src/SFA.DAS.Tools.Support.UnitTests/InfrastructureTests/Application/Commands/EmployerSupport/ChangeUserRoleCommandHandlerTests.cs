using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Commands.EmployerSupport;

public class ChangeUserRoleCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid(
        ChangeUserRoleCommand command,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<ILogger<ChangeUserRoleCommandHandler>> mockLogger,
        ChangeUserRoleCommandHandler handler
        )
    {
        // Arrange
        mockApiClient.Setup(client => client.ChangeUserRole(It.Is<ChangeUserRoleRequest>(req =>
            req.HashedAccountId == command.HashedAccountId &&
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
        ChangeUserRoleCommand command,
        [Frozen] Mock<IToolsSupportApimService> mockApiClient,
        [Frozen] Mock<ILogger<ChangeUserRoleCommandHandler>> mockLogger,
        ChangeUserRoleCommandHandler handler
        )
    {
        // Arrange
        mockApiClient.Setup(client => client.ChangeUserRole(It.IsAny<ChangeUserRoleRequest>(), It.IsAny<CancellationToken>()))
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

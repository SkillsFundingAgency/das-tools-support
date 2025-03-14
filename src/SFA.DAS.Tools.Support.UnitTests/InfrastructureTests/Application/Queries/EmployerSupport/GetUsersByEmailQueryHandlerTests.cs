using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.EmployerSupport;

public class GetUsersByEmailQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetUsersByEmailQueryResult_WhenCalled(
        GetUsersByEmailQuery query,
        GetMatchingUsersResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        GetUsersByEmailQueryHandler handler)
    {
        employerSupportApiClient.Setup(o => o.GetMatchingUsers(It.Is<string>(p => p == query.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Users.Should().BeEquivalentTo(response.Users);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldNotReturnAListOfUsersWhenEmailIsNull(
        GetMatchingUsersResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        GetUsersByEmailQueryHandler handler)
    {
        var query = new GetUsersByEmailQuery { Email = null };
        employerSupportApiClient.Setup(o => o.GetMatchingUsers(It.Is<string>(p => p == query.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Users.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldNotReturnAListOfUsersWhenEmailIsEmptySpace(
        GetMatchingUsersResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        GetUsersByEmailQueryHandler handler)
    {
        var query = new GetUsersByEmailQuery { Email = "     " };
        employerSupportApiClient.Setup(o => o.GetMatchingUsers(It.Is<string>(p => p == query.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Users.Should().BeNull();
    }

}

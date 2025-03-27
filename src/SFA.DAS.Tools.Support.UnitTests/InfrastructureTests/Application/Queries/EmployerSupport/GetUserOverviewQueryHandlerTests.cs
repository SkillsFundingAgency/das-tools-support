using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.EmployerSupport;

public class GetUserOverviewQueryHandlerTests
{

    [Test, MoqAutoData]
    public async Task Handle_ShouldReturn_GetUserOverviewQueryResult_WhenCalled(
        GetUserOverviewQuery query,
        GetUserOverviewResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        GetUserOverviewQueryHandler handler)
    {
        employerSupportApiClient.Setup(o => o.GetUserOverview(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(response);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldCallEmployerSupportApiClient_WithCorrectParameters(
       GetUserOverviewQuery query,
       GetUserOverviewResponse response,
       [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
       GetUserOverviewQueryHandler handler)
    {
        employerSupportApiClient.Setup(o => o.GetUserOverview(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        employerSupportApiClient.Verify(client => client.GetUserOverview(query.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

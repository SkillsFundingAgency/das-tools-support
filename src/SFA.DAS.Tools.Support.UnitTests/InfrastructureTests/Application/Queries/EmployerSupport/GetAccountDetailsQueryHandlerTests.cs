using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetAccountDetailsQueryResult_WhenCalled(
        GetAccountDetailsQuery query,
        GetAccountDetailsResponse response,
        [Frozen] Mock<IToolsSupportOuterApiClient> employerSupportApiClient,
        GetAccountDetailsQueryHandler handler)
    {
        employerSupportApiClient.Setup(o => o.GetAccountDetails(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEquivalentTo(response);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldCallEmployerSupportApiClientWithCorrectParameters(
        GetAccountDetailsQuery query,
        GetAccountDetailsResponse response,
        [Frozen] Mock<IToolsSupportOuterApiClient> employerSupportApiClient,
        GetAccountDetailsQueryHandler handler)
    {
        employerSupportApiClient.Setup(o => o.GetAccountDetails(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        employerSupportApiClient.Verify(client => client.GetAccountDetails(It.IsAny<long>(), query.AccountFieldsSelection.ToString(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

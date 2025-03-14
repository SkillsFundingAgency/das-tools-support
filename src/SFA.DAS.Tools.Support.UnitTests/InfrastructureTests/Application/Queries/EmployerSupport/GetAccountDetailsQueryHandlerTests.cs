using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Encoding;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.EmployerSupport;

public class GetAccountDetailsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetAccountDetailsQueryResult_WhenCalled(
        long accountId,
        GetAccountDetailsQuery query,
        GetAccountDetailsResponse response,
        [Frozen] Mock<IEncodingService> encodingService,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        GetAccountDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Decode(query.HashedAccountId, EncodingType.AccountId))
            .Returns(accountId);

        employerSupportApiClient.Setup(o => o.GetAccountDetails(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response).Verifiable();

        var result = await handler.Handle(query, CancellationToken.None);

        employerSupportApiClient.Verify();
        result.Should().BeEquivalentTo(response);
    }
}

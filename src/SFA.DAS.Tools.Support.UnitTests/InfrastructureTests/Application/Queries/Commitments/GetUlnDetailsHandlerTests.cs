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

public class GetUlnDetailsHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnUlnDetailsQueryResult_WhenCalled(
        GetUlnDetailsQuery query,
        GetMatchingApprenticeshipsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetUlnDetailsQueryHandler handler)
    {
        encodingService.Setup(x => x.Encode(It.IsAny<long>(), EncodingType.AccountId)).Returns("XXXXX");

        employerSupportApiClient.Setup(o => o.GetMatchingUlns(query.Uln, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Apprenticeships.Should().BeEquivalentTo(response.ApprovedApprenticeships);
    }
}

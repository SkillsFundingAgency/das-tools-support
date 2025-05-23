﻿using System.Collections.Generic;
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
using SFA.DAS.Tools.Support.Infrastructure.Services;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests.Application.Queries.EmployerSupport;

public class GetEmployerAccountsQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetEmployerAccountsQueryResult_WhenCalledWithValidPublicHashedAccountId(
        long accountId,
        GetEmployerAccountsQuery query,
        GetMatchingEmployerAccountsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetEmployerAccountsQueryHandler handler)
    {
        query.PayeRef = null;
        query.EmployerName = null;

        encodingService.Setup(x => x.TryDecode(It.IsAny<string>(), It.IsAny<EncodingType>(), out accountId))
            .Returns(true);

        employerSupportApiClient.Setup(o => o.GetMatchingAccounts(accountId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Accounts.Should().BeEquivalentTo(response.EmployerAccounts);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetEmployerAccountsQueryResult_WhenCalledWithPayeRef(
        GetEmployerAccountsQuery query,
        GetMatchingEmployerAccountsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetEmployerAccountsQueryHandler handler)
    {
        query.PublicHashedAccountId = null;
        query.EmployerName = null;

        employerSupportApiClient.Setup(o => o.GetMatchingAccounts(null, query.PayeRef, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Accounts.Should().BeEquivalentTo(response.EmployerAccounts);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldReturnGetEmployerAccountsQueryResult_WhenCalledWithEmployerName(
        GetEmployerAccountsQuery query,
        GetMatchingEmployerAccountsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetEmployerAccountsQueryHandler handler)
    {
        query.PublicHashedAccountId = null;
        query.PayeRef = null;
        query.EmployerName = "Test Employer";

        employerSupportApiClient.Setup(o => o.GetMatchingAccounts(null, null, query.EmployerName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Accounts.Should().BeEquivalentTo(response.EmployerAccounts);
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldNotReturnAListOfUsersWhenAllFieldsAreNull(
        GetMatchingEmployerAccountsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetEmployerAccountsQueryHandler handler)
    {
        var query = new GetEmployerAccountsQuery();

        employerSupportApiClient.Setup(o => o.GetMatchingAccounts(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Accounts.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Handle_ShouldPassANullAccountIdToAPI_WhenCalledWithAnInvalidPublicHashedAccountId(
        long accountId,
        GetEmployerAccountsQuery query,
        GetMatchingEmployerAccountsResponse response,
        [Frozen] Mock<IToolsSupportApimService> employerSupportApiClient,
        [Frozen] Mock<IEncodingService> encodingService,
        GetEmployerAccountsQueryHandler handler)
    {
        query.PayeRef = null;
        query.EmployerName = null;
        response.EmployerAccounts = new List<EmployerAccount>();

        encodingService.Setup(x => x.TryDecode(It.IsAny<string>(), It.IsAny<EncodingType>(), out accountId))
            .Returns(false);

        employerSupportApiClient.Setup(o => o.GetMatchingAccounts(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Accounts.Count.Should().Be(0);
        result.IsValidAccountCode.Should().BeFalse();
    }
}
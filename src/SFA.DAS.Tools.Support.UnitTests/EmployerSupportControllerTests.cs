using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests;

[TestFixture]
public class EmployerSupportControllerTests
{

    [Test, MoqAutoData]
    public async Task EmployerUserSearch_WithNoSearchValues_ShouldCallViewWithNoMatchingUsersList
    (
        GetUsersByEmailQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
    )
    {
        result.Users = null;

        mockMediator.Setup(m => m.Send(It.Is<GetUsersByEmailQuery>(q => q.Email == null), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.EmployerUserSearch(null) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(new EmployerUserSearchModel
        {
            Email = null,
            Users = null
        });
    }

    [Test, MoqAutoData]
    public async Task EmployerUserSearch_WithSearchValueExists_ShouldCallViewWithMatchingUsers
    (
        string email,
        GetUsersByEmailQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
    )
    {
        mockMediator.Setup(m => m.Send(It.Is<GetUsersByEmailQuery>(q => q.Email == email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.EmployerUserSearch(email) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        var model = response.Model as EmployerUserSearchModel;
        model.Email.Should().Be(email);
        model.Users.Should().BeEquivalentTo(result.Users.Select(x => new MatchedUser
            {Id = x.Id.ToString(), DisplayName = x.DisplayName, Email = x.Email}).ToList());
    }

    [Test, MoqAutoData]
    public async Task EmployerAccountsSearch_WithNoSearchValues_ShouldCallViewWithNoMatchingAccountsList
    (
        GetEmployerAccountsQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
    )
    {
        result.Accounts = null;

        mockMediator.Setup(m => m.Send(It.Is<GetEmployerAccountsQuery>(q => q.PublicHashedAccountId == null && q.PayeRef == null), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.EmployerAccountSearch(null, null) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(new EmployerAccountSearchModel
        {
            PublicHashedId = null,
            Accounts = null
        });
    }

    [Test, MoqAutoData]
    public async Task EmployerAccountsSearch_WithPublicHashedAccountSearchValueExists_ShouldCallViewWithMatchingAccounts
    (
        string publicHashedId,
        GetEmployerAccountsQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
    )
    {
        mockMediator.Setup(m => m.Send(It.Is<GetEmployerAccountsQuery>(q => q.PublicHashedAccountId == publicHashedId && q.PayeRef == null), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.EmployerAccountSearch(publicHashedId, null) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        var model = response.Model as EmployerAccountSearchModel;
        model.PublicHashedId.Should().Be(publicHashedId);
        model.PayeRef.Should().BeNull();
        model.Accounts.Should().BeEquivalentTo(result.Accounts.Select(x => new MatchedAccount
            { AccountId = x.AccountId, DasAccountName = x.DasAccountName, PublicHashedAccountId = x.PublicHashedAccountId, HashedAccountId = x.HashedAccountId}).ToList());
    }

    [Test, MoqAutoData]
    public async Task EmployerAccountsSearch_WithPayeSearchValueExists_ShouldCallViewWithMatchingAccounts
    (
        string paye,
        GetEmployerAccountsQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
    )
    {
        mockMediator.Setup(m => m.Send(It.Is<GetEmployerAccountsQuery>(q => q.PublicHashedAccountId == null && q.PayeRef == paye), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var response = await controller.EmployerAccountSearch(null, paye) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        var model = response.Model as EmployerAccountSearchModel;
        model.PublicHashedId.Should().BeNull();
        model.PayeRef.Should().Be(paye);
        model.Accounts.Should().BeEquivalentTo(result.Accounts.Select(x => new MatchedAccount
            { AccountId = x.AccountId, DasAccountName = x.DasAccountName, PublicHashedAccountId = x.PublicHashedAccountId, HashedAccountId = x.HashedAccountId }).ToList());
    }
}

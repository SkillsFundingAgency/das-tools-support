using System;
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
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests;

[TestFixture]
public class EmployerSupportControllerTests
{
    [Test, MoqAutoData]
    public async Task AccountDetails_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        AccountFieldsSelection accountFieldsSelection,
        GetAccountDetailsQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        var viewModel = AccountDetailsViewModel.MapFrom(result);
        viewModel.SelectedTab = accountFieldsSelection;

        mockMediator.Setup(m => m.Send(It.Is<GetAccountDetailsQuery>(q => q.HashedAccountId == hashedAccountId && q.AccountFieldsSelection == accountFieldsSelection), default))
                     .ReturnsAsync(result);

        // Act
        var response = await controller.AccountDetails(hashedAccountId, accountFieldsSelection) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(viewModel);
    }

    [Test, MoqAutoData]
    public async Task AccountDetails_ShouldCallMediatorWithCorrectQuery(
         string hashedAccountId,
       AccountFieldsSelection accountFieldsSelection,
       GetAccountDetailsQueryResult result,
       [Frozen] Mock<IMediator> mockMediator,
       [Greedy] EmployerSupportController controller
        )
    {
        // Arrange

        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        mockMediator.Setup(m => m.Send(It.IsAny<GetAccountDetailsQuery>(), default))
                     .ReturnsAsync(result);

        // Act
        await controller.AccountDetails(hashedAccountId, accountFieldsSelection);

        // Assert
        mockMediator.Verify(m => m.Send(It.Is<GetAccountDetailsQuery>(q => q.HashedAccountId == hashedAccountId && q.AccountFieldsSelection == accountFieldsSelection), default), Times.Once);
    }


    [Test, MoqAutoData]
    public async Task UserOverview_ShouldReturnViewWithViewModel_WhenCalled(
        Guid userId,
        GetUserOverviewQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        var query = new GetUserOverviewQuery
        {
            UserId = userId
        };

        var viewModel = UserOverviewViewModel.MapFrom(result);

        mockMediator.Setup(m => m.Send(It.Is<GetUserOverviewQuery>(q => q.UserId == userId), default))
                     .ReturnsAsync(result);
       
        // Act
        var response = await controller.UserOverview(userId) as ViewResult;

        // Assert
        mockMediator.Verify(m => m.Send(It.Is<GetUserOverviewQuery>(q => q.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(viewModel);
    }
}

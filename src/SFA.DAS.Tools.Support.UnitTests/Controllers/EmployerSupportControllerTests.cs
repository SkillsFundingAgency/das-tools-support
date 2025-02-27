using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

public class EmployerSupportControllerTests
{
    [Test, MoqAutoData]
    public async Task When_Calling_Index_And_User_Is_Not_EmployerSupportUser_Then_Redirect_To_Support_Index(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider)
    {
        authorizationProvider.Setup(m => m.IsEmployerSupportAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);

        var controller = new EmployerSupportController(authorizationProvider.Object, mediatorMock.Object);
        var result = await controller.Index();

        var actualResult = (RedirectToActionResult)result;
        actualResult.ControllerName.Should().Be("Support");
        actualResult.ActionName.Should().Be("Index");
    }

    [Test, MoqAutoData]
    public async Task When_Calling_Index_And_User_Is_EmployerSupportUser_Then_Return_View(
        [Frozen] Mock<IMediator> mediatorMock,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider)
    {
        authorizationProvider.Setup(m => m.IsEmployerSupportAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(true);

        var controller = new EmployerSupportController(authorizationProvider.Object, mediatorMock.Object);
        var result = await controller.Index();

        result.Should().BeOfType<ViewResult>();
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
                     .ReturnsAsync(result).Verifiable();

        // Act
        var response = await controller.UserOverview(userId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(viewModel);
    }
}
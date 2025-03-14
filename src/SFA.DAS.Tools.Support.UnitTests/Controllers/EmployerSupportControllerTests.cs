using System;
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
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

public class EmployerSupportControllerTests
{
    [Test, MoqAutoData]
    public async Task UserOverview_ShouldReturnViewWithViewModel_WhenCalled(
       Guid userId,
       GetUserOverviewQueryResult result,
       [Frozen] Mock<IMediator> mockMediator,
       [Greedy] EmployerSupportController controller
       )
    {
        var viewModel = (UserOverviewViewModel)result;

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
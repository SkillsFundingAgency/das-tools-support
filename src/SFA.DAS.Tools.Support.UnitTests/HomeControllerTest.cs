using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.Home;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.UnitTests;

public class HomeControllerTest
{
    [Test, DomainAutoData]
    public async Task Index_Returns_ViewModel(bool useDfESignIn,
        [Frozen] IAuthorizationService authorizationService,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = useDfESignIn;
        var controller = new HomeController(config, authorizationService)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };

        //sut
        var result = await controller.Index();

        //assert
        result.Should().NotBeNull();
        var resultModel = result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<HomeIndexViewModel>().Which;

        resultModel.UseDfESignIn.Should().Be(useDfESignIn);
    }

    [Test, DomainAutoData]
    public async Task When_Tier3_User_Authenticated_Index_Returns_Redirect_To_Support_Index(
        string userName,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        var authorizationService = new Mock<IAuthorizationService>();

        authorizationService
            .Setup(m => m.AuthorizeAsync(
                authorizedUser,
                It.IsAny<object>(),
                PolicyNames.HasTier3Account))
            .ReturnsAsync(AuthorizationResult.Success());

        var controller = new HomeController(config, authorizationService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
        };

        //sut
        var result = await controller.Index();

        //assert
        result.Should().NotBeNull();
        var actualResult = (RedirectToActionResult)result;
        actualResult.ControllerName.Should().Be("Support");
        actualResult.ActionName.Should().Be("Index");
    }
    
    [Test, DomainAutoData]
    public async Task When_User_Authenticated_And_SupportConsole_Feature_Disabled_Index_Returns_Redirect_To_Support_Index(
        string userName,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;
        config.EnableSupportConsoleFeature = false;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        var authorizationService = new Mock<IAuthorizationService>();

        authorizationService
            .Setup(m => m.AuthorizeAsync(
                authorizedUser,
                It.IsAny<object>(),
                PolicyNames.HasTier3Account))
            .ReturnsAsync(AuthorizationResult.Failed);

        var controller = new HomeController(config, authorizationService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
        };

        //sut
        var result = await controller.Index();

        //assert
        result.Should().NotBeNull();
        var actualResult = (RedirectToActionResult)result;
        actualResult.ControllerName.Should().Be("Support");
        actualResult.ActionName.Should().Be("Index");
    }
    
    [Test, DomainAutoData]
    public async Task When_Non_Tier3_User_Authenticated_And_SupportConsole_Feature_Enabled_Index_Returns_Redirect_To_EmployerSupport_Index(
        string userName,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        var authorizationService = new Mock<IAuthorizationService>();

        authorizationService
            .Setup(m => m.AuthorizeAsync(
                authorizedUser,
                It.IsAny<object>(),
                PolicyNames.HasTier3Account))
            .ReturnsAsync(AuthorizationResult.Failed);

        var controller = new HomeController(config, authorizationService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
        };

        //sut
        var result = await controller.Index();

        //assert
        result.Should().NotBeNull();
        var actualResult = (RedirectToActionResult)result;
        actualResult.ControllerName.Should().Be("EmployerSupport");
        actualResult.ActionName.Should().Be("Index");
    }
}
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.Home;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

public class HomeControllerTest
{
    [Test, DomainAutoData]
    public async Task Index_Returns_ViewModel(bool useDfESignIn,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = useDfESignIn;
        var controller = new HomeController(config, authorizationProvider.Object)
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
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(authorizedUser)).ReturnsAsync(true);

        var controller = new HomeController(config, authorizationProvider.Object)
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
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;
        config.EnableSupportConsoleFeature = false;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(authorizedUser)).ReturnsAsync(true);

        var controller = new HomeController(config, authorizationProvider.Object)
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
    public async Task When_SupportConsole_Feature_Disabled_Index_Returns_Redirect_To_Support_Index(
        string userName,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(authorizedUser)).ReturnsAsync(false);

        var controller = new HomeController(config, authorizationProvider.Object)
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
    public async Task When_SupportConsole_Feature_Enabled_And_User_IsEmployerSupportOnlyAuthorized_True_Index_Returns_Redirect_To_EmployerSupport_Index(
        string userName,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsEmployerSupportOnlyAuthorized(authorizedUser)).ReturnsAsync(true);

        var controller = new HomeController(config, authorizationProvider.Object)
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
    
    [Test, DomainAutoData]
    public async Task When_SupportConsole_Feature_Enabled_And_User_IsEmployerSupportOnlyAuthorized_False_Index_Returns_Redirect_To_Support_Index(
        string userName,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.UseDfESignIn = true;
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsEmployerSupportTier1Authorized(authorizedUser)).ReturnsAsync(false);

        var controller = new HomeController(config, authorizationProvider.Object)
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
}
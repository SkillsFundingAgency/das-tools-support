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

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

public class HomeControllerTest
{
    [Test, DomainAutoData]
    public async Task Index_Returns_ViewModel(
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() },
        };

        //sut
        var result = await controller.Index();

        //assert
        result.Should().NotBeNull();
    }

    [Test, DomainAutoData]
    public async Task When_Tier3_User_Authenticated_Index_Returns_Redirect_To_Support_Index(
        string userName,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
                [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,

        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(authorizedUser)).ReturnsAsync(true);

        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
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
                [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,

        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.EnableSupportConsoleFeature = false;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(authorizedUser)).ReturnsAsync(true);

        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
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
                [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,

        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(authorizedUser)).ReturnsAsync(false);

        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
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
    public async Task When_SupportConsole_Feature_Enabled_And_User_IsEmployerSupportOnlyAuthorized_True_Index_Returns_Redirect_To_EmployerSupport_EmployerSearch(
        string userName,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
                [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,

        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsEmployerSupportOnlyAuthorized(authorizedUser)).ReturnsAsync(true);

        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
        };

        //sut
        var result = await controller.Index();

        //assert
        result.Should().NotBeNull();
        var actualResult = (RedirectToActionResult)result;
        actualResult.ControllerName.Should().Be("EmployerSupport");
        actualResult.ActionName.Should().Be("EmployerUserSearch");
    }

    [Test, DomainAutoData]
    public async Task When_SupportConsole_Feature_Enabled_And_User_IsEmployerSupportOnlyAuthorized_False_Index_Returns_Redirect_To_Support_Index(
        string userName,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] ToolsSupportConfig config)
    {
        //arrange
        config.EnableSupportConsoleFeature = true;

        var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity([new Claim("name", userName)], "mock"));
        var httpContext = new Mock<HttpContext>();

        httpContext.Setup(c => c.User).Returns(authorizedUser);

        authorizationProvider.Setup(m => m.IsEmployerSupportTier1OrHigherAuthorized(authorizedUser)).ReturnsAsync(false);

        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
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
    public void When_SignOut_Called_Session_Is_Cleared_And_SignOutResult_Returned(
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider,
        [Frozen] Mock<IHttpContextAccessor> httpContextAccessor,
        [Frozen] Mock<ISession> sessionMock,
        [Frozen] ToolsSupportConfig config)
    {
        var httpContext = new Mock<HttpContext>();
        httpContext.Setup(c => c.Session).Returns(sessionMock.Object);
        httpContextAccessor.Setup(a => a.HttpContext).Returns(httpContext.Object);

        var controller = new HomeController(config, authorizationProvider.Object, httpContextAccessor.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
        };

        var result = controller.SignOut();

        sessionMock.Verify(s => s.Clear(), Times.Once(), "Session should be cleared on signout");

        result.Should().NotBeNull();
        var signOutResult = result.Should().BeOfType<SignOutResult>().Subject;
    }
}
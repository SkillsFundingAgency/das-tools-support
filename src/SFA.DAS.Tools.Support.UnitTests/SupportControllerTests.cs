using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.UnitTests;

public class SupportControllerTests
{
    [Test, DomainAutoData]
    public async Task PostLogin_ReturnsView_And_HasTier3AccountPermission_True(
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider
        )
    {
        authorizationProvider.Setup(m => m.IsPrivilegeAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(true);

        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns("ConfigValue");

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection(It.Is<string>(k => k == "BaseUrl"))).Returns(mockSection.Object);

        var sc = new SupportController(authorizationProvider.Object);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeTrue();
    }

    [Theory, DomainAutoData]
    public async Task PostLogin_ReturnsView_And_HasTier3AccountPermission_False(
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        authorizationProvider.Setup(m => m.IsPrivilegeAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);

        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns("ConfigValue");

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection(It.Is<string>(k => k == "BaseUrl"))).Returns(mockSection.Object);

        var sc = new SupportController(authorizationProvider.Object);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeFalse();
    }
    
    [Test, DomainAutoData]
    public async Task PostLogin_ReturnsView_And_HasSupportConsoleAccess_True_When_Tier(
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        authorizationProvider.Setup(m => m.IsPrivilegeAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);

        var mockSection = new Mock<IConfigurationSection>();
        mockSection.Setup(x => x.Value).Returns("ConfigValue");

        var mockConfig = new Mock<IConfiguration>();
        mockConfig.Setup(x => x.GetSection(It.Is<string>(k => k == "BaseUrl"))).Returns(mockSection.Object);

        var sc = new SupportController(authorizationProvider.Object);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeFalse();
    }
}
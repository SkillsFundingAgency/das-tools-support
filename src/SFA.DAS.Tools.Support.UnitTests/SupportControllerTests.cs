using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.UnitTests;

public class SupportControllerTests
{
    [Test, DomainAutoData]
    public async Task PostLogin_ReturnsView_And_HasTier3AccountPermission_True(
        [Frozen] ToolsSupportConfig config,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider
        )
    {
        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(true);
        
        var sc = new SupportController(authorizationProvider.Object, config);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeTrue();
    }

    [Theory, DomainAutoData]
    public async Task PostLogin_ReturnsView_And_HasTier3AccountPermission_False(
        [Frozen] ToolsSupportConfig config,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);
        
        var sc = new SupportController(authorizationProvider.Object, config);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeFalse();
    }
    
    [Test, DomainAutoData]
    public async Task PostLogin_ReturnsView_And_HasSupportConsoleAccess_True_When_Tier(
        [Frozen] ToolsSupportConfig config,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);
        
        var sc = new SupportController(authorizationProvider.Object, config);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeFalse();
    }
    
    [Test]
    [MoqInlineAutoData(true)]
    [MoqInlineAutoData(false)]
    public async Task PostLogin_ReturnsView_And_HasEmployerSupportAccount_Correlates_To_IsEmployerSupportAuthorized(
        bool hasEmployerSupportAccount,
        [Frozen] ToolsSupportConfig config,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        authorizationProvider.Setup(m => m.IsEmployerSupportAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(hasEmployerSupportAccount);
        
        var sc = new SupportController(authorizationProvider.Object, config);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasEmployerSupportAccount.Should().Be(hasEmployerSupportAccount);
    }
    
    [Test, DomainAutoData]
    public async Task PostLogin_Redirects_To_EmployerSupport_Index_When_EnableSupportConsoleFeature_Is_True_And_User_IsEmployerSupportOnlyAuthorized(
        [Frozen] ToolsSupportConfig config,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        config.EnableSupportConsoleFeature = true;
        authorizationProvider.Setup(m => m.IsEmployerSupportOnlyAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(true);
        
        var sc = new SupportController(authorizationProvider.Object, config);
        var result = await sc.Index();

        var actualResult = (RedirectToActionResult)result;
        actualResult.ControllerName.Should().Be("EmployerSupport");
        actualResult.ActionName.Should().Be("Index");
    }
    
    [Test, DomainAutoData]
    public async Task PostLogin_Redirects_To_EmployerSupport_Index_When_EnableSupportConsoleFeature_Is_True_And_User_Is_Not_IsEmployerSupportOnlyAuthorized(
        [Frozen] ToolsSupportConfig config,
        [Frozen] Mock<IAuthorizationProvider> authorizationProvider )
    {
        config.EnableSupportConsoleFeature = true;
        authorizationProvider.Setup(m => m.IsEmployerSupportOnlyAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);
        authorizationProvider.Setup(m => m.IsPauseOrResumeApprenticeshipAuthorized(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(false);
        
        var sc = new SupportController(authorizationProvider.Object, config);
        var result = await sc.Index();

        var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
        resultModel.HasTier3Account.Should().BeFalse();
    }
}
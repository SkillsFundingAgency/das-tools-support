using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class SupportControllerTests
    {
        [Test, DomainAutoData]
        public async Task PostLogin_ReturnsView_And_HasTier3AccountPermission_True(ILogger<SupportController> logger)
        {
            Mock<IAuthorizationService> authorizationService = new Mock<IAuthorizationService>();

            authorizationService
                .Setup(
                    m => m.AuthorizeAsync(
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<object>(),
                        PolicyNames.HasTier3Account))
                .ReturnsAsync(
                    AuthorizationResult.Success());

            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("ConfigValue");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection(It.Is<string>(k => k == "BaseUrl"))).Returns(mockSection.Object);

            SupportController sc = new SupportController(logger, mockConfig.Object, authorizationService.Object);
            var result = await sc.Index();

            var resultModel = result.Should().BeOfType<ViewResult>().
            Which.Model.Should().BeOfType<IndexViewModel>().Which;
            resultModel.HasTier3Account.Should().BeTrue();
        }

        [Theory, DomainAutoData]
        public async Task PostLogin_ReturnsView_And_HasTier3AccountPermission_False(ILogger<SupportController> logger)
        {
            Mock<IAuthorizationService> authorizationService = new Mock<IAuthorizationService>();

            authorizationService
                .Setup(
                    m => m.AuthorizeAsync(
                        It.IsAny<ClaimsPrincipal>(),
                        It.IsAny<object>(),
                        PolicyNames.HasTier3Account))
                .ReturnsAsync(
                    AuthorizationResult.Failed());

            Mock<IConfigurationSection> mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(x => x.Value).Returns("ConfigValue");

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection(It.Is<string>(k => k == "BaseUrl"))).Returns(mockSection.Object);

            SupportController sc = new SupportController(logger, mockConfig.Object, authorizationService.Object);
            var result = await sc.Index();

            var resultModel = result.Should().BeOfType<ViewResult>().
                Which.Model.Should().BeOfType<IndexViewModel>().Which;
            resultModel.HasTier3Account.Should().BeFalse();
        }
    }
}
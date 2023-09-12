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

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class HomeControllerTest
    {
        [Test, DomainAutoData]
        public void Index_Returns_ViewModel(
            bool useDfESignIn,
            [Frozen] IOptions<DfESignInConfig> dfESignInConfig)
        {
            //arrange
            dfESignInConfig.Value.UseDfESignIn = useDfESignIn;
            var controller = new HomeController(dfESignInConfig)
            {
                ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() },
            };

            //sut
            var result = controller.Index();

            //assert
            result.Should().NotBeNull();
            var resultModel = result.Should().BeOfType<ViewResult>().
                Which.Model.Should().BeOfType<HomeIndexViewModel>().Which;
            resultModel.UseDfESignIn.Should().Be(useDfESignIn);
        }

        [Test, DomainAutoData]
        public void When_User_Authenticated_Index_Returns_Redirect(
            string userName,
            [Frozen] IOptions<DfESignInConfig> dfESignInConfig)
        {
            //arrange
            dfESignInConfig.Value.UseDfESignIn = true;
            var authorizedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("name", userName)
            }, "mock"));
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(c => c.User).Returns(authorizedUser);
            var controller = new HomeController(dfESignInConfig)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext.Object }
            };

            //sut
            var result = controller.Index();

            //assert
            result.Should().NotBeNull();
            var actualResult = (RedirectToActionResult)result;
            actualResult.ControllerName.Should().Be("Support");
            actualResult.ActionName.Should().Be("Index");
        }
    }
}

using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.Home;

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

            //sut
            var controller = new HomeController(dfESignInConfig);
            var result = controller.Index();

            //assert
            result.Should().NotBeNull();
            var resultModel = result.Should().BeOfType<ViewResult>().
                Which.Model.Should().BeOfType<HomeIndexViewModel>().Which;
            resultModel.UseDfESignIn.Should().Be(useDfESignIn);
        }
    }
}

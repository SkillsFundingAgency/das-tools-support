using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.Error;
using System;
using FluentAssertions;

namespace SFA.DAS.Tools.Support.UnitTests;

[TestFixture]
public class ErrorControllerTests
{
    private ErrorController _sut;
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<IOptions<DfESignInConfig>> _mockDfESignInOptions;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockDfESignInOptions = new Mock<IOptions<DfESignInConfig>>();
    }

    [TearDown]
    public void TearDown() => _sut?.Dispose();

    [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", true)]
    [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service", false)]
    public void WhenStatusCodeIs403Then403ViewIsReturned(string env, string helpLink, bool useDfESignIn)
    {
        //arrange
        var mockDfESignInConfig = new DfESignInConfig { UseDfESignIn = useDfESignIn };
        _mockDfESignInOptions.Setup(x => x.Value).Returns(mockDfESignInConfig);
        _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
        _mockConfiguration.Setup(x => x["UseDfESignIn"]).Returns(Convert.ToString(useDfESignIn));
        _sut = new ErrorController(_mockDfESignInOptions.Object, _mockConfiguration.Object);

        //sut
        var result = (ViewResult)_sut.AccessDenied();

        //assert
        result.Should().NotBeNull();

        var actualModel = result.Model as Error403ViewModel;
        actualModel?.HelpPageLink.Should().Be(helpLink);
        actualModel?.UseDfESignIn.Should().Be(useDfESignIn);
    }
}
﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.Error;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;

[TestFixture]
public class ErrorControllerTests
{
    private ErrorController _sut;
    private Mock<IConfiguration> _mockConfiguration;

    [SetUp]
    public void SetUp()
    {
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [TearDown]
    public void TearDown() => _sut?.Dispose();

    [TestCase("test", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    [TestCase("pp", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    [TestCase("local", "https://test-services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    [TestCase("prd", "https://services.signin.education.gov.uk/approvals/select-organisation?action=request-service")]
    public void WhenStatusCodeIs403Then403ViewIsReturned(string env, string helpLink)
    {
        //arrange
        var mockDfESignInConfig = new ToolsSupportConfig();
        _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns(env);
        _sut = new ErrorController(_mockConfiguration.Object);

        //sut
        var result = (ViewResult)_sut.AccessDenied();

        //assert
        result.Should().NotBeNull();

        var actualModel = result.Model as Error403ViewModel;
        actualModel?.HelpPageLink.Should().Be(helpLink);
    }
}
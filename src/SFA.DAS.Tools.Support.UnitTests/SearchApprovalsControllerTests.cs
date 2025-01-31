using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.UnitTests;

public class SearchApprovalsControllerTests
{
    private const string StopAction = "stop";
    private const string ResumeAction = "resume";

    [Test, DomainAutoData]
    public async Task SearchApprenticeships_GET_WithNoParameters_ReturnsView(ILogger<SearchApprovalsController> logger,
        IEmployerCommitmentsService employerCommitmentsService,
        IMapper mapper,
        IOptions<ClaimsConfiguration> claimConfiguration)
    {
        //When

        var authorizationService = new Mock<IAuthorizationService>();

        authorizationService
            .Setup(
                m => m.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    PolicyNames.Privileged))
            .ReturnsAsync(
                AuthorizationResult.Success());

        var sut = new SearchApprovalsController(logger, employerCommitmentsService, mapper,
            claimConfiguration, authorizationService.Object);
        //var request = new ApprovalSearchApprenticeshipRequest(null, null, null, null, null, null, null, null, StopAction);
        var request = new ApprovalSearchApprenticeshipRequest { Act = StopAction };
        var result = await sut.SearchApprenticeships(request);

        //Then
        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<SearchApprenticeshipsViewModel>().Which.Should().BeEquivalentTo(new SearchApprenticeshipsViewModel());
    }

    [Test, DomainAutoData]
    public async Task SearchApprenticeships_GET_WithNoParameters_And_Unauthorized_Action_ReturnsForbid(ILogger<SearchApprovalsController> logger,
        IEmployerCommitmentsService employerCommitmentsService,
        IMapper mapper,
        IOptions<ClaimsConfiguration> claimConfiguration)
    {
        //When
        var authorizationService = new Mock<IAuthorizationService>();

        authorizationService
            .Setup(
                m => m.AuthorizeAsync(
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<object>(),
                    It.IsAny<string>()))
            .ReturnsAsync(
                AuthorizationResult.Failed);

        var sut = new SearchApprovalsController(logger, employerCommitmentsService, mapper, claimConfiguration, authorizationService.Object);

        var request = new ApprovalSearchApprenticeshipRequest { Act = ResumeAction };
        var result = await sut.SearchApprenticeships(request);

        //Then
        result.Should().BeAssignableTo<ForbidResult>();
    }
}
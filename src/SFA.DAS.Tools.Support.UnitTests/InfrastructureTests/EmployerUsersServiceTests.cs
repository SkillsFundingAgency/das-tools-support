using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Requests;
using SFA.DAS.Tools.Support.Infrastructure.OuterApi.Responses;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests;

public class EmployerUsersServiceTests
{
    [Test, DomainAutoData]
    public async Task WhenSuspendingUser_ShouldSuspendUser(
        [Frozen]Mock<IToolsSupportApimService> toolsSupportApimService,
        SuspendUserRequest request,
        EmployerUsersService service)
    {
        toolsSupportApimService
            .Setup(x => x.SuspendEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeUserStatusResponse()).Verifiable();

        await service.SuspendUser(request, CancellationToken.None);

        toolsSupportApimService.VerifyAll();
    }

    [Test, DomainAutoData]
    public async Task WhenSuspendingUser_ShouldReturnSuspendedUserResult_WithNoErrors(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        SuspendUserRequest request,
        EmployerUsersService service)
    {
        toolsSupportApimService
            .Setup(x => x.SuspendEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeUserStatusResponse());

        var result = await service.SuspendUser(request, CancellationToken.None);

        result.HasError.Should().BeFalse();
    }

    [Test, DomainAutoData]
    public async Task WhenSuspendingUser_WhenError_Returned_In_Api_Result_ShouldReturnSuspendedUserResult_WithErrors(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        SuspendUserRequest request,
        EmployerUsersService service)
    {
        toolsSupportApimService
            .Setup(x => x.SuspendEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeUserStatusResponse
            {
                Errors = new Dictionary<string, string> { { "errorTitle", "error" } }
            });

        var result = await service.SuspendUser(request, CancellationToken.None);

        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("errorTitle error");
    }

    [Test, DomainAutoData]
    public async Task WhenSuspendingUser_WhenException_Thrown_By_ApiClient_ShouldReturnSuspendedUserResult_WithErrors(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        SuspendUserRequest request,
        EmployerUsersService service)
    {
        const string apiExceptionMessage = "Api threw exception";
        toolsSupportApimService
            .Setup(x => x.SuspendEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception(apiExceptionMessage));

        var result = await service.SuspendUser(request, CancellationToken.None);

        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(apiExceptionMessage);
    }

    [Test, DomainAutoData]
    public async Task WhenResumingUser_ShouldResumeUser(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        ResumeUserRequest request,
        EmployerUsersService service)
    {
        toolsSupportApimService
            .Setup(x => x.ResumeEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeUserStatusResponse()).Verifiable();

        await service.ResumeUser(request, CancellationToken.None);

        toolsSupportApimService.VerifyAll();
    }

    [Test, DomainAutoData]
    public async Task WhenResumingUser_ShouldReturnResumedUserResult_WithNoErrors(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        ResumeUserRequest request,
        EmployerUsersService service)
    {
        toolsSupportApimService
            .Setup(x => x.ResumeEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeUserStatusResponse());

        var result = await service.ResumeUser(request, CancellationToken.None);

        result.HasError.Should().BeFalse();
    }

    [Test, DomainAutoData]
    public async Task WhenResumingUser_WhenError_Returned_In_Api_Result_ShouldReturnResumedUserResult_WithErrors(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        ResumeUserRequest request,
        EmployerUsersService service)
    {
        toolsSupportApimService
            .Setup(x => x.ResumeEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ChangeUserStatusResponse
            {
                Errors = new Dictionary<string, string> { { "errorTitle", "error" } }
            });

        var result = await service.ResumeUser(request, CancellationToken.None);

        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be("errorTitle error");
    }

    [Test, DomainAutoData]
    public async Task WhenResumingUser_WhenException_Thrown_By_ApiClient_ShouldReturnResumedUserResult_WithErrors(
        [Frozen] Mock<IToolsSupportApimService> toolsSupportApimService,
        ResumeUserRequest request,
        EmployerUsersService service)
    {
        const string apiExceptionMessage = "Api threw exception";
        toolsSupportApimService
            .Setup(x => x.ResumeEmployerUser(request.UserId, It.IsAny<ChangeUserStatusRequest>(), It.IsAny<CancellationToken>()))
            .Throws(new Exception(apiExceptionMessage));

        var result = await service.ResumeUser(request, CancellationToken.None);

        result.HasError.Should().BeTrue();
        result.ErrorMessage.Should().Be(apiExceptionMessage);
    }
}
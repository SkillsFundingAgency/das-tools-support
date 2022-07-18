using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using Xunit;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class EmployerUsersServiceTests
    {
        [Theory, AutoMoqData]
        public async Task WhenSuspendingUser_ShouldSuspendUser(
            [Frozen]Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            SuspendUserRequest request,
            EmployerUsersService service)
        {
            employerUsersApiClientMock.Setup(x => x.SuspendUser(request.UserId, It.IsAny<ChangedByUserInfo>())).ReturnsAsync(new SuspendUserResponse()).Verifiable();

            var result = await service.SuspendUser(request, new CancellationToken());

            employerUsersApiClientMock.VerifyAll();
        }

        [Theory, AutoMoqData]
        public async Task WhenSuspendingUser_ShouldReturnSuspendedUserResult_WithNoErrors(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            SuspendUserRequest request,
            EmployerUsersService service)
        {
            employerUsersApiClientMock.Setup(x => x.SuspendUser(request.UserId, It.IsAny<ChangedByUserInfo>())).ReturnsAsync(new SuspendUserResponse());

            var result = await service.SuspendUser(request, new CancellationToken());

            result.HasError.Should().BeFalse();
        }

        [Theory, AutoMoqData]
        public async Task WhenSuspendingUser_WhenError_Returned_In_Api_Result_ShouldReturnSuspendedUserResult_WithErrors(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            SuspendUserRequest request,
            EmployerUsersService service)
        {
            employerUsersApiClientMock.Setup(x => x.SuspendUser(request.UserId, It.IsAny<ChangedByUserInfo>())).ReturnsAsync(new SuspendUserResponse
            {
                Errors = new Dictionary<string, string> { { "errorTitle", "error" } }
            });

            var result = await service.SuspendUser(request, new CancellationToken());

            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("errorTitle error");
        }

        [Theory, AutoMoqData]
        public async Task WhenSuspendingUser_WhenException_Thrown_By_ApiClient_ShouldReturnSuspendedUserResult_WithErrors(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            SuspendUserRequest request,
            EmployerUsersService service)
        {
            const string apiExceptionMessage = "Api threw exception";
            employerUsersApiClientMock.Setup(x => x.SuspendUser(request.UserId, It.IsAny<ChangedByUserInfo>())).Throws(new WebException(apiExceptionMessage));

            var result = await service.SuspendUser(request, new CancellationToken());

            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be(apiExceptionMessage);
        }

        [Theory, AutoMoqData]
        public async Task WhenResumingUser_ShouldResumeUser(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            ResumeUserRequest request,
            EmployerUsersService service)
        {
            employerUsersApiClientMock.Setup(x => x.ResumeUser(request.UserId, It.IsAny<ChangedByUserInfo>())).ReturnsAsync(new ResumeUserResponse()).Verifiable();

            var result = await service.ResumeUser(request, new CancellationToken());

            employerUsersApiClientMock.VerifyAll();
        }

        [Theory, AutoMoqData]
        public async Task WhenResumingUser_ShouldReturnResumedUserResult_WithNoErrors(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            ResumeUserRequest request,
            EmployerUsersService service)
        {
            employerUsersApiClientMock.Setup(x => x.ResumeUser(request.UserId, It.IsAny<ChangedByUserInfo>())).ReturnsAsync(new ResumeUserResponse());

            var result = await service.ResumeUser(request, new CancellationToken());

            result.HasError.Should().BeFalse();
        }

        [Theory, AutoMoqData]
        public async Task WhenResumingUser_WhenError_Returned_In_Api_Result_ShouldReturnResumedUserResult_WithErrors(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            ResumeUserRequest request,
            EmployerUsersService service)
        {
            employerUsersApiClientMock.Setup(x => x.ResumeUser(request.UserId, It.IsAny<ChangedByUserInfo>())).ReturnsAsync(new ResumeUserResponse
            {
                Errors = new Dictionary<string, string> { { "errorTitle", "error" } }
            });

            var result = await service.ResumeUser(request, new CancellationToken());

            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be("errorTitle error");
        }

        [Theory, AutoMoqData]
        public async Task WhenResumingUser_WhenException_Thrown_By_ApiClient_ShouldReturnResumedUserResult_WithErrors(
            [Frozen] Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            ResumeUserRequest request,
            EmployerUsersService service)
        {
            const string apiExceptionMessage = "Api threw exception";
            employerUsersApiClientMock.Setup(x => x.ResumeUser(request.UserId, It.IsAny<ChangedByUserInfo>())).Throws(new WebException(apiExceptionMessage));

            var result = await service.ResumeUser(request, new CancellationToken());

            result.HasError.Should().BeTrue();
            result.ErrorMessage.Should().Be(apiExceptionMessage);
        }
    }
}
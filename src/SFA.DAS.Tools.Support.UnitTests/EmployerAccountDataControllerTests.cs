using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static SFA.DAS.Tools.Support.Web.Models.StopApprenticeshipRow;
namespace SFA.DAS.Tools.Support.UnitTests
{
    public class EmployerAccountDataControllerTests
    {

        [Theory, AutoMoqData]
        public async Task SearchUsers_POST_EmptyAccountId_ReturnsError(EmployerAccountDataController sut)
        {
            //Given
            var accountId = string.Empty;

            //When
            var result = await sut.Index(accountId);

            //Then
            result.Should().BeOfType<JsonResult>().Which
                .Value.Should().BeEquivalentTo(new
                {
                    ErrorTitle = "Invalid Search",
                    ErrorMessage = "Account Id must be populated"
                });
        }

        [Theory, AutoMoqData]
        public async Task SearchUsers_POST_ApiCallErrors_ReturnsError([Frozen] Mock<IEmployerAccountsService> api, GetAccountUsersResult apiResult, EmployerAccountDataController sut)
        {
            //Given
            var accountId = "hash1";
            apiResult.ErrorMessage = "Api Error Message";

            api.Setup(s => s.GetAccountUsers(
                It.Is<GetAccountUsersRequest>(s => s.AccountId == accountId),
                It.IsAny<CancellationToken>()))
             .Returns(Task.FromResult(apiResult));

            //When
            var result = await sut.Index(accountId);

            //Then
            result.Should().BeOfType<JsonResult>().Which
                .Value.Should().BeEquivalentTo(new
                {
                    ErrorTitle = "Call to Employer Accounts Api Failed",
                    ErrorMessage = apiResult.ErrorMessage
                });
        }

        [Theory, AutoMoqData]
        public async Task SearchUsers_POST_ApiCallSucceeds_ReturnsResult([Frozen] Mock<IEmployerAccountsService> api, GetAccountUsersResult apiResult, EmployerAccountDataController sut)
        {
            //Given
            var accountId = "hash1";
            apiResult.ErrorMessage = string.Empty;
            
            api.Setup(s => s.GetAccountUsers(
               It.Is<GetAccountUsersRequest>(s => s.AccountId == accountId),
               It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(apiResult));

            //When
            var result = await sut.Index(accountId);

            //Then
            result.Should().BeOfType<JsonResult>().Which
                .Value.Should().BeEquivalentTo(apiResult.Users.Select(u => new
                {
                    u.Email,
                    u.Name,
                    u.Role,                
                    u.UserRef
                }));
        }

    }
}

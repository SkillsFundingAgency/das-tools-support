using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using Xunit;
using FluentAssertions;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class EmployerAccountServiceTests
    {
        [Theory, AutoMoqData]
        public async Task GetAccountByInternalAccountId_ReturnedResultIsMapped(EmployerAccountsService service)
        {
            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                InternalAccountId = 12345
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.Users);

            result.Users.First().Should().BeEquivalentTo(new
            {
                Name = "Test",
                Email = "t@est.com",
                Role = "emperor",
                Status = EAS.Account.Api.Types.InvitationStatus.Accepted,
                UserRef = "123",
                CanReceiveNotifications = true
            });            
        }

        [Theory, AutoMoqData]
        public async Task PassingBothHashIdAndInternalAccountId_FailsValidation(EmployerAccountsService service)
        {
            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                AccountId = "1234",
                InternalAccountId = 12345
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.ErrorMessage);
            result.ErrorMessage.Should().Be("Request failed validation");
        }
    }
}
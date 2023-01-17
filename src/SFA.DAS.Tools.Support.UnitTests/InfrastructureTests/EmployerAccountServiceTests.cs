using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerUsers.Api.Client;
using SFA.DAS.EmployerUsers.Api.Types;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;

namespace SFA.DAS.Tools.Support.UnitTests
{
    public class EmployerAccountServiceTests
    {
        [Test, DomainAutoData]
        public async Task GetAccountByInternalAccountId_ReturnedResultIsMapped(EmployerAccountUsersService service)
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
                Status = InvitationStatus.Accepted,
                UserRef = "123",
                CanReceiveNotifications = true
            });            
        }

        [Test, DomainAutoData]
        public async Task PassingBothHashIdAndInternalAccountId_FailsValidation(EmployerAccountUsersService service)
        {
            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                HashedAccountId = "1234",
                InternalAccountId = 12345
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.ErrorMessage);
            result.ErrorMessage.Should().Be("Request failed validation");
        }

        [Test, DomainAutoData]
        public async Task ShouldMapUserAccountStatusFromEmployerUsers_Active(
            long internalAccountId,
            ICollection<TeamMemberViewModel> teamMembers,
            ICollection<UserViewModel> employerUsers,
            Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            Mock<IAccountApiClient> accountsApiMock,
            EmployerAccountUsersService service)
        {
            accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
            employerUsers.First().IsLocked = false;
            employerUsers.First().IsSuspended = false;

            _ = teamMembers.Zip(employerUsers, (TeamMemberViewModel teamMember, UserViewModel employerUser) =>
            {
                employerUser.Id = teamMember.UserRef;
                employerUsersApiClientMock.Setup(m => m.GetUserById(teamMember.UserRef)).ReturnsAsync(employerUser);
                return employerUser;
            }).ToList();

            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                InternalAccountId = internalAccountId
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.Users);
            result.Users.First().AccountStatus = "Active";
        }

        [Test, DomainAutoData]
        public async Task ShouldMapUserAccountStatusFromEmployerUsers_Suspended(
            long internalAccountId,
            ICollection<TeamMemberViewModel> teamMembers,
            ICollection<UserViewModel> employerUsers,
            Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            Mock<IAccountApiClient> accountsApiMock,
            EmployerAccountUsersService service)
        {
            accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
            employerUsers.First().IsLocked = false;
            employerUsers.First().IsSuspended = true;

            _ = teamMembers.Zip(employerUsers, (TeamMemberViewModel teamMember, UserViewModel employerUser) =>
            {
                employerUser.Id = teamMember.UserRef;
                employerUsersApiClientMock.Setup(m => m.GetUserById(teamMember.UserRef)).ReturnsAsync(employerUser);
                return employerUser;
            }).ToList();

            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                InternalAccountId = internalAccountId
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.Users);
            result.Users.First().AccountStatus = "Suspended";
        }

        [Test, DomainAutoData]
        public async Task ShouldMapUserAccountStatusFromEmployerUsers_Locked(
           long internalAccountId,
           ICollection<TeamMemberViewModel> teamMembers,
           ICollection<UserViewModel> employerUsers,
           Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
           Mock<IAccountApiClient> accountsApiMock,
           EmployerAccountUsersService service)
        {
            accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
            employerUsers.First().IsLocked = true;
            employerUsers.First().IsSuspended = false;

            _ = teamMembers.Zip(employerUsers, (TeamMemberViewModel teamMember, UserViewModel employerUser) =>
            {
                employerUser.Id = teamMember.UserRef;
                employerUsersApiClientMock.Setup(m => m.GetUserById(teamMember.UserRef)).ReturnsAsync(employerUser);
                return employerUser;
            }).ToList();

            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                InternalAccountId = internalAccountId
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.Users);
            result.Users.First().AccountStatus = "Locked";
        }

        [Test, DomainAutoData]
        public async Task ShouldMapUserAccountStatusFromEmployerUsers_SuspendedAndLocked(
            long internalAccountId,
            ICollection<TeamMemberViewModel> teamMembers,
            ICollection<UserViewModel> employerUsers,
            Mock<IEmployerUsersApiClient> employerUsersApiClientMock,
            Mock<IAccountApiClient> accountsApiMock,
            EmployerAccountUsersService service)
        {
            accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
            employerUsers.First().IsLocked = true;
            employerUsers.First().IsSuspended = true;

            _ = teamMembers.Zip(employerUsers, (TeamMemberViewModel teamMember, UserViewModel employerUser) =>
            {
                employerUser.Id = teamMember.UserRef;
                employerUsersApiClientMock.Setup(m => m.GetUserById(teamMember.UserRef)).ReturnsAsync(employerUser);
                return employerUser;
            }).ToList();

            var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
            {
                InternalAccountId = internalAccountId
            }, new System.Threading.CancellationToken());

            Assert.NotNull(result.Users);
            result.Users.First().AccountStatus = "Locked";
        }
    }
}
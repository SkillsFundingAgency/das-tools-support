using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.UnitTests.AutoFixture;
using InvitationStatus = SFA.DAS.EAS.Account.Api.Types.InvitationStatus;

namespace SFA.DAS.Tools.Support.UnitTests.InfrastructureTests;

public class EmployerAccountServiceTests
{
    [Test, DomainAutoData]
    public async Task GetAccountByInternalAccountId_ReturnedResultIsMapped(EmployerAccountUsersService service)
    {
        var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
        {
            InternalAccountId = 12345
        });

        result.Users.Should().NotBeNull();

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
        });

        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Be("Request failed validation");
    }

    [Test, DomainAutoData]
    public async Task ShouldMapUserAccountStatusFromEmployerUsers_Active(
        long internalAccountId,
        ICollection<TeamMemberViewModel> teamMembers,
        ICollection<UserProfile> employerUsers,
        Mock<IToolsSupportApimService> toolsSupportApimServiceMock,
        Mock<IAccountApiClient> accountsApiMock,
        EmployerAccountUsersService service)
    {
        accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
        employerUsers.First().IsLocked = false;
        employerUsers.First().IsSuspended = false;

        _ = teamMembers.Zip(employerUsers, (teamMember, employerUser) =>
        {
            employerUser.GovUkIdentifier = teamMember.UserRef;
            var response = new GetUserByUserRefResponse { User = employerUser };
            toolsSupportApimServiceMock.Setup(m => m.GetUserByUserRef(teamMember.UserRef, CancellationToken.None)).ReturnsAsync(response);
            return employerUser;
        }).ToList();

        var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
        {
            InternalAccountId = internalAccountId
        });

        result.Users.Should().NotBeNull();
        result.Users.First().AccountStatus.Should().Be("Active");
    }

    [Test, DomainAutoData]
    public async Task ShouldMapUserAccountStatusFromEmployerUsers_Suspended(
        long internalAccountId,
        ICollection<TeamMemberViewModel> teamMembers,
        ICollection<UserProfile> employerUsers,
        Mock<IToolsSupportApimService> toolsSupportApimServiceMock,
        Mock<IAccountApiClient> accountsApiMock,
        EmployerAccountUsersService service)
    {
        accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
        employerUsers.First().IsLocked = false;
        employerUsers.First().IsSuspended = true;

        _ = teamMembers.Zip(employerUsers, (teamMember, employerUser) =>
        {
            employerUser.GovUkIdentifier = teamMember.UserRef;
            var response = new GetUserByUserRefResponse { User = employerUser };
            toolsSupportApimServiceMock.Setup(m => m.GetUserByUserRef(teamMember.UserRef, CancellationToken.None)).ReturnsAsync(response);
            return employerUser;
        }).ToList();

        var result = await service.GetAccountUsers(new GetAccountUsersRequest()
        {
            InternalAccountId = internalAccountId
        });

        result.Users.Should().NotBeNull();
        result.Users.First().AccountStatus.Should().Be("Suspended");
    }

    [Test, DomainAutoData]
    public async Task ShouldMapUserAccountStatusFromEmployerUsers_Locked(
        long internalAccountId,
        ICollection<TeamMemberViewModel> teamMembers,
        ICollection<UserProfile> employerUsers,
        Mock<IToolsSupportApimService> toolsSupportApimServiceMock,
        Mock<IAccountApiClient> accountsApiMock,
        EmployerAccountUsersService service)
    {
        accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
        employerUsers.First().IsLocked = true;
        employerUsers.First().IsSuspended = false;

        _ = teamMembers.Zip(employerUsers, (teamMember, employerUser) =>
        {
            employerUser.GovUkIdentifier = teamMember.UserRef;
            var response = new GetUserByUserRefResponse { User = employerUser };
            toolsSupportApimServiceMock.Setup(m => m.GetUserByUserRef(teamMember.UserRef, CancellationToken.None)).ReturnsAsync(response);
            return employerUser;
        }).ToList();

        var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
        {
            InternalAccountId = internalAccountId
        });

        result.Users.Should().NotBeNull();
        result.Users.First().AccountStatus.Should().Be("Locked");
    }

    [Test, DomainAutoData]
    public async Task ShouldMapUserAccountStatusFromEmployerUsers_SuspendedAndLocked(
        long internalAccountId,
        ICollection<TeamMemberViewModel> teamMembers,
        ICollection<UserProfile> employerUsers,
        Mock<IToolsSupportApimService> toolsSupportApimServiceMock,
        Mock<IAccountApiClient> accountsApiMock,
        EmployerAccountUsersService service)
    {
        accountsApiMock.Setup(m => m.GetAccountUsers(internalAccountId)).ReturnsAsync(teamMembers);
        employerUsers.First().IsLocked = true;
        employerUsers.First().IsSuspended = true;

        _ = teamMembers.Zip(employerUsers, (teamMember, employerUser) =>
        {
            employerUser.GovUkIdentifier = teamMember.UserRef;
            var response = new GetUserByUserRefResponse { User = employerUser };
            toolsSupportApimServiceMock.Setup(m => m.GetUserByUserRef(teamMember.UserRef, CancellationToken.None))
                .ReturnsAsync(response);
            return employerUser;
        }).ToList();

        var result = await service.GetAccountUsers(new Core.Models.GetAccountUsersRequest()
        {
            InternalAccountId = internalAccountId
        });

        result.Users.Should().NotBeNull();
        result.Users.First().AccountStatus.Should().Be("Locked");
    }
}
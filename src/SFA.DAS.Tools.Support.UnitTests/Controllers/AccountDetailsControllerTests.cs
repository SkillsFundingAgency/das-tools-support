using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models.Challenge;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChallengeEntry;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountOrganisations;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetChallengePermission;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetFinanceDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetTeamMembers;
using SFA.DAS.Tools.Support.Infrastructure.Cache;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests.Controllers;
[TestFixture]
public class AccountDetailsControllerTests
{
    [Test, MoqAutoData]
    public async Task Organisations_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        Account accountData,
        GetAccountOrganisationsQueryResult organisationsResult,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        var query = new GetAccountOrganisationsQuery
        {
            HashedAccountId = hashedAccountId
        };

        mockMediator.Setup(m => m.Send(It.Is<GetAccountOrganisationsQuery>(q => q.HashedAccountId == hashedAccountId), default))
            .ReturnsAsync(organisationsResult)
            .Verifiable();

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        var expectedViewModel = new OrganisationViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccount,
            LegalEntities = organisationsResult.LegalEntities
        };

        // Act
        var response = await controller.Organisations(hashedAccountId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task TeamMembers_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        Account accountData,
        GetTeamMembersQueryResult teamMemberResult,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        var query = new GetTeamMembersQuery
        {
            HashedAccountId = hashedAccountId
        };

        mockMediator.Setup(m => m.Send(It.Is<GetTeamMembersQuery>(q => q.HashedAccountId == hashedAccountId), default))
            .ReturnsAsync(teamMemberResult)
            .Verifiable();

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        var expectedViewModel = new TeamMembersViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
            TeamMembers = teamMemberResult.TeamMembers
        };

        // Act
        var response = await controller.TeamMembers(hashedAccountId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task Finance_ShouldReturnViewWithViewModel_WhenUserIsNotTier1(
        string hashedAccountId,
        Account accountData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Frozen] Mock<IAuthorizationProvider> mockAuthProvider,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        var financeData = new GetFinanceDetailsQueryResult
        {
            Balance = 100,
            PayeSchemes = [],
            Transactions = []
        };

        mockAuthProvider.Setup(a => a.IsEmployerSupportTier1OnlyAuthorized(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(false)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<GetFinanceDetailsQuery>(q => q.HashedAccountId == hashedAccountId), default))
            .ReturnsAsync(financeData)
            .Verifiable();

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        var expectedViewModel = new FinanceViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance,
            PayeSchemes = financeData.PayeSchemes,
            Transactions = financeData.Transactions,
            Balance = financeData.Balance
        };

        // Act
        var response = await controller.Finance(hashedAccountId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockAuthProvider.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();
        mockAuthProvider.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task Finance_ShouldRedirectToChallenge_WhenUserIsTier1AndFormNotCompleted(
        string hashedAccountId,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Frozen] Mock<IAuthorizationProvider> mockAuthProvider,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        var user = CreateUser();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        mockAuthProvider.Setup(a => a.IsEmployerSupportTier1OnlyAuthorized(user))
             .ReturnsAsync(true)
             .Verifiable();

        var cacheKey = $"FinanceChallenge_{hashedAccountId}_{user.Identity.Name}";
        mockCacheService.Setup(c => c.RetrieveFromCache<bool>(cacheKey))
            .ReturnsAsync(false)
            .Verifiable();

        // Act
        var response = await controller.Finance(hashedAccountId) as RedirectToActionResult;

        // Assert
        mockAuthProvider.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();
        mockAuthProvider.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ActionName.Should().Be(RouteNames.Account_Challenge);
        response.RouteValues["hashedAccountId"].Should().Be(hashedAccountId);
    }

    [Test, MoqAutoData]
    public async Task Finance_ShouldReturnViewWithViewModel_WhenUserIsTier1AndFormCompleted(
        string hashedAccountId,
        Account accountData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Frozen] Mock<IAuthorizationProvider> mockAuthProvider,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        var financeData = new GetFinanceDetailsQueryResult
        {
            Balance = 100,
            PayeSchemes = [],
            Transactions = []
        };
        var user = CreateUser();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        mockAuthProvider.Setup(a => a.IsEmployerSupportTier1OnlyAuthorized(user))
            .ReturnsAsync(true)
            .Verifiable();


        var cacheKey = $"FinanceChallenge_{hashedAccountId}_{user.Identity.Name}";
        mockCacheService.Setup(c => c.RetrieveFromCache<bool>(cacheKey))
            .ReturnsAsync(true)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<GetFinanceDetailsQuery>(q => q.HashedAccountId == hashedAccountId), default))
            .ReturnsAsync(financeData)
            .Verifiable();

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        var expectedViewModel = new FinanceViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance,
            PayeSchemes = financeData.PayeSchemes,
            Transactions = financeData.Transactions,
            Balance = financeData.Balance
        };

        // Act
        var response = await controller.Finance(hashedAccountId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockAuthProvider.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();
        mockAuthProvider.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task InviteTeamMember_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        Account accountData,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        var expectedViewModel = new InvitationViewModel
        {
            HashedAccountId = hashedAccountId,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        // Act
        var response = await controller.InviteTeamMember(hashedAccountId) as ViewResult;

        // Assert
        mockCacheService.Verify();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task InviteTeamMember_ShouldCallCacheServiceWithCorrectKey_WhenCalled(
        string hashedAccountId,
        Account accountData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        // Act
        await controller.InviteTeamMember(hashedAccountId);

        // Assert
        mockCacheService.Verify();
        mockCacheService.VerifyNoOtherCalls();
        mockMediator.VerifyNoOtherCalls();
    }

    [Test, MoqAutoData]
    public async Task InviteTeamMember_ShouldReturnConfirmationViewWithViewModel_WhenModelStateIsValid(
        InvitationViewModel invitationModel,
        SendTeamMemberInviteCommandResult result,
        Account accountData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        controller.ModelState.Clear();

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{invitationModel.HashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<SendTeamMemberInviteCommand>(c =>
                c.HashedAccountId == invitationModel.HashedAccountId &&
                c.FullName == invitationModel.FullName &&
                c.Email == invitationModel.Email &&
                c.Role == invitationModel.Role), default))
            .ReturnsAsync(result)
            .Verifiable();

        var expectedViewModel = new TeamMemberActionConfirmationViewModel
        {
            HashedAccountId = invitationModel.HashedAccountId,
            Success = result.Success,
            MemberEmail = invitationModel.Email,
            TeamMemberAction = TeamMemberAction.InviteNewTeamMember,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        // Act
        var response = await controller.InviteTeamMember(invitationModel) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.Verify();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be("TeamMemberActionConfirmation");
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task InviteTeamMember_ShouldReturnOriginalView_WhenModelStateIsInvalid(
        InvitationViewModel invitationModel,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        controller.ModelState.AddModelError("FullName", "Full Name is required");

        // Act
        var response = await controller.InviteTeamMember(invitationModel) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.Model.Should().BeSameAs(invitationModel);
    }

    [Test, MoqAutoData]
    public async Task ResendInvitation_ShouldReturnConfirmationViewWithViewModel_WhenParametersAreValid(
        string hashedAccountId,
        string email,
        Account accountData,
        ResendTeamMemberInvitationCommandResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<ResendTeamMemberInvitationCommand>(c =>
                c.HashedAccountId == hashedAccountId &&
                c.Email == email), default))
            .ReturnsAsync(result)
            .Verifiable();

        var expectedViewModel = new TeamMemberActionConfirmationViewModel
        {
            HashedAccountId = hashedAccountId,
            Success = result.Success,
            MemberEmail = Uri.UnescapeDataString(email),
            TeamMemberAction = TeamMemberAction.ResendInvitation,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        // Act
        var response = await controller.ResendInvitation(hashedAccountId, email) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be("TeamMemberActionConfirmation");
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task ResendInvitation_ShouldReturnTeamMembersView_WhenParametersAreInvalid(
        string hashedAccountId,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Act
        var response = await controller.ResendInvitation(hashedAccountId, "") as ViewResult;

        // Assert
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.Account_TeamMembers);
        response.Model.Should().BeEquivalentTo(new { hashedAccountId });
    }

    [Test, MoqAutoData]
    public async Task PayeSchemeDeclarations_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        string childId,
        string obscuredPayeRef,
        Account accountData,
        GetPayeSchemeLevyDeclarationsResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<GetPayeSchemeLevyDeclarationsQuery>(q =>
                q.HashedAccountId == hashedAccountId &&
                q.HashedPayeRef == childId), default))
            .ReturnsAsync(result)
            .Verifiable();

        var expectedViewModel = new PayeSchemeLevyDeclarationViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance,
            PayeSchemeName = result.PayeSchemeName,
            PayeSchemeFormatedAddedDate = result.PayeSchemeFormatedAddedDate,
            PayeSchemeRef = obscuredPayeRef,
            LevyDeclarations = result.LevyDeclarations,
            UnexpectedError = result.UnexpectedError
        };

        // Act
        var response = await controller.PayeSchemeDeclarations(hashedAccountId, childId, obscuredPayeRef) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task ChangeUserRole_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        Role role,
        string email,
        string fullName,
        Account accountData,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        var expectedViewModel = new ChangeUserRoleViewModel
        {
            HashedAccountId = hashedAccountId,
            Email = email,
            Role = role,
            FullName = fullName,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        // Act
        var response = await controller.ChangeUserRole(hashedAccountId, role, email, fullName) as ViewResult;

        // Assert
        mockCacheService.Verify();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task ChangeUserRole_ShouldReturnConfirmationViewWithViewModel_WhenModelStateIsValid(
        ChangeUserRoleViewModel changeUserRoleViewModel,
        ChangeUserRoleCommandResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        controller.ModelState.Clear();

        var decodedEmail = Uri.UnescapeDataString(changeUserRoleViewModel.Email);

        mockMediator.Setup(m => m.Send(It.Is<ChangeUserRoleCommand>(c =>
                c.HashedAccountId == changeUserRoleViewModel.HashedAccountId &&
                c.Email == decodedEmail &&
                c.Role == changeUserRoleViewModel.Role), default))
            .ReturnsAsync(result)
            .Verifiable();

        var expectedViewModel = new TeamMemberActionConfirmationViewModel
        {
            HashedAccountId = changeUserRoleViewModel.HashedAccountId,
            Success = result.Success,
            MemberEmail = decodedEmail,
            TeamMemberAction = TeamMemberAction.ChangeUserRole,
            Role = changeUserRoleViewModel.Role,
            Account = changeUserRoleViewModel.Account,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        // Act
        var response = await controller.ChangeUserRole(changeUserRoleViewModel) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be("TeamMemberActionConfirmation");
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task ChangeUserRole_ShouldReturnOriginalView_WhenModelStateIsInvalid(
        ChangeUserRoleViewModel changeUserRoleViewModel,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        controller.ModelState.AddModelError("Email", "Email is required");

        // Act
        var response = await controller.ChangeUserRole(changeUserRoleViewModel) as ViewResult;

        // Assert
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeSameAs(changeUserRoleViewModel);
    }

    [Test, MoqAutoData]
    public async Task Challenge_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        Account accountData,
        GetChallengePermissionQueryResult challengeData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{hashedAccountId}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<GetChallengePermissionQuery>(q =>
                q.HashedAccountId == hashedAccountId), default))
            .ReturnsAsync(challengeData)
            .Verifiable();

        var expectedViewModel = new ChallengeViewModel
        {
            Id = hashedAccountId,
            Characters = challengeData.Characters,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance
        };

        // Act
        var response = await controller.Challenge(hashedAccountId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task Challenge_ShouldRedirectToFinance_WhenModelStateIsValidAndResponseIsValid(
        ChallengeEntry challengeEntry,
        ChallengeEntryCommandResult response,
        Account accountData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        controller.ModelState.Clear();
        response.IsValid = true;
        var user = CreateUser();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{challengeEntry.Id}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<ChallengeEntryCommand>(c =>
                c.HashedAccountId == challengeEntry.Id &&
                c.Challenge1 == challengeEntry.Challenge1 &&
                c.Challenge2 == challengeEntry.Challenge2 &&
                c.Balance == challengeEntry.Balance &&
                c.FirstCharacterPosition == challengeEntry.FirstCharacterPosition &&
                c.SecondCharacterPosition == challengeEntry.SecondCharacterPosition), default))
            .ReturnsAsync(response)
            .Verifiable();

        mockCacheService.Setup(c => c.SetAsync(
                It.Is<string>(key => key == $"FinanceChallenge_{challengeEntry.Id}_{user.Identity.Name}"),
                true,
                1))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var responseResult = await controller.Challenge(challengeEntry) as RedirectToActionResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        responseResult.Should().NotBeNull();
        responseResult.ActionName.Should().Be(RouteNames.Account_Finance);
        responseResult.RouteValues["hashedAccountId"].Should().Be(challengeEntry.Id);
    }

    [Test, MoqAutoData]
    public async Task Challenge_ShouldReturnViewWithError_WhenModelStateIsValidAndResponseIsInvalid(
        ChallengeEntry challengeEntry,
        ChallengeEntryCommandResult response,
        Account accountData,
        [Frozen] Mock<IMediator> mockMediator,
        [Frozen] Mock<ICacheService> mockCacheService,
        [Greedy] AccountDetailsController controller)
    {
        // Arrange
        controller.ModelState.Clear();
        response.IsValid = false;
        var user = CreateUser();

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        mockCacheService.Setup(c => c.GetOrSetAsync(
                It.Is<string>(key => key == $"AccountDetails_{challengeEntry.Id}"),
                It.IsAny<Func<Task<Account>>>(),
                It.IsAny<int>()))
            .ReturnsAsync(accountData)
            .Verifiable();

        mockMediator.Setup(m => m.Send(It.Is<ChallengeEntryCommand>(c =>
                c.HashedAccountId == challengeEntry.Id &&
                c.Challenge1 == challengeEntry.Challenge1 &&
                c.Challenge2 == challengeEntry.Challenge2 &&
                c.Balance == challengeEntry.Balance &&
                c.FirstCharacterPosition == challengeEntry.FirstCharacterPosition &&
                c.SecondCharacterPosition == challengeEntry.SecondCharacterPosition), default))
            .ReturnsAsync(response)
            .Verifiable();

        var expectedViewModel = new ChallengeViewModel
        {
            Characters = response.Characters,
            Id = challengeEntry.Id,
            HasError = true,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance
        };

        // Act
        var responseResult = await controller.Challenge(challengeEntry) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockCacheService.Verify();
        mockMediator.VerifyNoOtherCalls();
        mockCacheService.VerifyNoOtherCalls();

        responseResult.Should().NotBeNull();
        responseResult.ViewName.Should().BeNull();
        responseResult.Model.Should().BeEquivalentTo(expectedViewModel);
    }

    private static ClaimsPrincipal CreateUser()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "TESTNAME")
        };
        var identity = new ClaimsIdentity(claims, "mock");
        return new ClaimsPrincipal(identity);
    }
}

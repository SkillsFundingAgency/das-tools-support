using System;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Controllers;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.UnitTests;

[TestFixture]
public class EmployerSupportControllerTests
{
    [Test, MoqAutoData]
    public async Task AccountDetails_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        AccountFieldsSelection accountFieldsSelection,
        GetAccountDetailsQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        var viewModel = AccountDetailsViewModel.MapFrom(result);
        viewModel.SelectedTab = accountFieldsSelection;

        mockMediator.Setup(m => m.Send(It.Is<GetAccountDetailsQuery>(q => q.HashedAccountId == hashedAccountId && q.AccountFieldsSelection == accountFieldsSelection), default))
                     .ReturnsAsync(result);

        // Act
        var response = await controller.AccountDetails(hashedAccountId, accountFieldsSelection) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(viewModel);
    }

    [Test, MoqAutoData]
    public async Task AccountDetails_ShouldCallMediatorWithCorrectQuery(
         string hashedAccountId,
       AccountFieldsSelection accountFieldsSelection,
       GetAccountDetailsQueryResult result,
       [Frozen] Mock<IMediator> mockMediator,
       [Greedy] EmployerSupportController controller
        )
    {
        // Arrange

        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        mockMediator.Setup(m => m.Send(It.IsAny<GetAccountDetailsQuery>(), default))
                     .ReturnsAsync(result);

        // Act
        await controller.AccountDetails(hashedAccountId, accountFieldsSelection);

        // Assert
        mockMediator.Verify(m => m.Send(It.Is<GetAccountDetailsQuery>(q => q.HashedAccountId == hashedAccountId && q.AccountFieldsSelection == accountFieldsSelection), default), Times.Once);
    }


    [Test, MoqAutoData]
    public async Task UserOverview_ShouldReturnViewWithViewModel_WhenCalled(
        Guid userId,
        GetUserOverviewQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        var query = new GetUserOverviewQuery
        {
            UserId = userId
        };

        var viewModel = UserOverviewViewModel.MapFrom(result);

        mockMediator.Setup(m => m.Send(It.Is<GetUserOverviewQuery>(q => q.UserId == userId), default))
                     .ReturnsAsync(result).Verifiable();

        // Act
        var response = await controller.UserOverview(userId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().BeNull();
        response.Model.Should().BeEquivalentTo(viewModel);
    }

    [Test, MoqAutoData]
    public void InviteTeamMember_ShouldReturnViewWithViewModel_WhenCalled(
        string hashedAccountId,
        GetUserOverviewQueryResult result,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Act
        var response = controller.InviteTeamMember(hashedAccountId) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();

        var model = response.Model as AccountDetailsViewModel;

        model.Should().NotBeNull();
        model.Account.Should().NotBeNull();
        model.Account.HashedAccountId.Should().Be(hashedAccountId);
        model.SelectedTab.Should().Be(AccountFieldsSelection.EmployerAccountTeam);
        model.InvitationViewModel.Should().NotBeNull();
        model.InvitationViewModel.HashedAccountId.Should().Be(hashedAccountId);
    }

    [Test, MoqAutoData]
    public async Task InviteTeamMember_ShouldReturnViewWithSuccess_WhenModelStateIsValid(
        InvitationViewModel invitationModel,
        SendTeamMemberInviteCommandResult commandResult,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        mockMediator.Setup(m => m.Send(It.Is<SendTeamMemberInviteCommand>(cmd =>
            cmd.HashedAccountId == invitationModel.HashedAccountId &&
            cmd.FullName == invitationModel.FullName &&
            cmd.Email == invitationModel.Email &&
            cmd.Role == invitationModel.Role), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult)
            .Verifiable();

        controller.ModelState.Clear();

        // Act
        var response = await controller.InviteTeamMember(invitationModel) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.EmployerSupport_AccountDetails);

        var model = response.Model as AccountDetailsViewModel;
        model.Should().NotBeNull();
        model.Account.HashedAccountId.Should().Be(invitationModel.HashedAccountId);
        model.SelectedTab.Should().Be(AccountFieldsSelection.EmployerAccountTeam);
        model.HasFormSubmittedSuccessfully.Should().BeTrue();
        model.InvitationViewModel.Should().BeNull();
        model.TeamMemberActionConfirmation.Should().NotBeNull();
        model.TeamMemberActionConfirmation.HashedAccountId.Should().Be(invitationModel.HashedAccountId);
        model.TeamMemberActionConfirmation.Success.Should().Be(commandResult.Success);
        model.TeamMemberActionConfirmation.MemberEmail.Should().Be(invitationModel.Email);
        model.TeamMemberActionConfirmation.TeamMemberAction.Should().Be(TeamMemberAction.InviteNewTeamMember);
    }

    [Test, MoqAutoData]
    public async Task InviteTeamMember_ShouldReturnViewWithErrors_WhenModelStateIsInvalid(
        InvitationViewModel invitationModel,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        controller.ModelState.AddModelError("Email", "Email is required.");

        // Act
        var response = await controller.InviteTeamMember(invitationModel) as ViewResult;

        // Assert
        mockMediator.Verify(m => m.Send(It.IsAny<SendTeamMemberInviteCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.EmployerSupport_AccountDetails);

        var model = response.Model as AccountDetailsViewModel;
        model.Should().NotBeNull();
        model.Account.HashedAccountId.Should().Be(invitationModel.HashedAccountId);
        model.SelectedTab.Should().Be(AccountFieldsSelection.EmployerAccountTeam);
        model.InvitationViewModel.Should().Be(invitationModel);
        model.HasFormSubmittedSuccessfully.Should().BeFalse();
        model.TeamMemberActionConfirmation.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task ResendInvitation_ShouldReturnViewWithSuccess_WhenQueryParametersAreValid(
        string hashedAccountId,
        string email,
        ResendTeamMemberInvitationCommandResult commandResult,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        mockMediator.Setup(m => m.Send(It.Is<ResendTeamMemberInvitationCommand>(cmd =>
            cmd.HashedAccountId == hashedAccountId &&
            cmd.Email == email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult)
            .Verifiable();

        // Act
        var response = await controller.ResendInvitation(hashedAccountId, email) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.EmployerSupport_AccountDetails);

        var model = response.Model as AccountDetailsViewModel;
        model.Should().NotBeNull();
        model.Account.HashedAccountId.Should().Be(hashedAccountId);
        model.SelectedTab.Should().Be(AccountFieldsSelection.EmployerAccountTeam);
        model.HasFormSubmittedSuccessfully.Should().BeTrue();
        model.InvitationViewModel.Should().BeNull();
        model.TeamMemberActionConfirmation.Should().NotBeNull();
        model.TeamMemberActionConfirmation.HashedAccountId.Should().Be(hashedAccountId);
        model.TeamMemberActionConfirmation.Success.Should().Be(commandResult.Success);
        model.TeamMemberActionConfirmation.MemberEmail.Should().Be(email);
        model.TeamMemberActionConfirmation.TeamMemberAction.Should().Be(TeamMemberAction.ResendInvitation);
    }

    [Test, MoqAutoData]
    public async Task ResendInvitation_ShouldReturnViewWithDefaultModel_WhenQueryParametersAreInvalid(
        string hashedAccountId,
        string email,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        hashedAccountId = null;
        email = null;

        // Act
        var response = await controller.ResendInvitation(hashedAccountId, email) as ViewResult;

        // Assert
        mockMediator.Verify(m => m.Send(It.IsAny<ResendTeamMemberInvitationCommand>(), It.IsAny<CancellationToken>()), Times.Never);

        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.EmployerSupport_AccountDetails);

        var model = response.Model as AccountDetailsViewModel;
        model.Should().NotBeNull();
        model.Account.HashedAccountId.Should().Be(hashedAccountId);
        model.SelectedTab.Should().Be(AccountFieldsSelection.EmployerAccountTeam);
        model.HasFormSubmittedSuccessfully.Should().BeFalse();
        model.InvitationViewModel.Should().BeNull();
        model.TeamMemberActionConfirmation.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void ChangeUserRole_ShouldReturnViewWithViewModel_WhenQueryParametersAreValid(
        string hashedAccountId,
        Role role,
        string email,
        string fullName,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        var expectedViewModel = new AccountDetailsViewModel
        {
            Account = new Account { HashedAccountId = hashedAccountId },
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
            ChangeUserRoleViewModel = new ChangeUserRoleViewModel
            {
                HashedAccountId = hashedAccountId,
                Email = email,
                Role = role,
                FullName = fullName
            }
        };

        // Act
        var response = controller.ChangUsereRole(hashedAccountId, role, email, fullName) as ViewResult;

        // Assert
        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.EmployerSupport_AccountDetails);

        var model = response.Model as AccountDetailsViewModel;
        model.Should().NotBeNull();
        model.Should().BeEquivalentTo(expectedViewModel);
    }

    [Test, MoqAutoData]
    public async Task ChangeUserRole_ShouldReturnViewWithSuccess_WhenModelStateIsValid(
        ChangeUserRoleViewModel changeUserRoleViewModel,
        ChangeUserRoleCommandResult commandResult,
        [Frozen] Mock<IMediator> mockMediator,
        [Greedy] EmployerSupportController controller
        )
    {
        // Arrange
        mockMediator.Setup(m => m.Send(It.Is<ChangeUserRoleCommand>(cmd =>
            cmd.HashedAccountId == changeUserRoleViewModel.HashedAccountId &&
            cmd.Email == Uri.UnescapeDataString(changeUserRoleViewModel.Email) &&
            cmd.Role == changeUserRoleViewModel.Role), It.IsAny<CancellationToken>()))
            .ReturnsAsync(commandResult)
            .Verifiable();

        controller.ModelState.Clear();

        // Act
        var response = await controller.ChangeUserRole(changeUserRoleViewModel) as ViewResult;

        // Assert
        mockMediator.Verify();
        mockMediator.VerifyNoOtherCalls();

        response.Should().NotBeNull();
        response.ViewName.Should().Be(RouteNames.EmployerSupport_AccountDetails);

        var model = response.Model as AccountDetailsViewModel;
        model.Should().NotBeNull();
        model.Account.HashedAccountId.Should().Be(changeUserRoleViewModel.HashedAccountId);
        model.SelectedTab.Should().Be(AccountFieldsSelection.EmployerAccountTeam);
        model.HasFormSubmittedSuccessfully.Should().BeTrue();
        model.ChangeUserRoleViewModel.Should().BeNull();
        model.TeamMemberActionConfirmation.Should().NotBeNull();
        model.TeamMemberActionConfirmation.HashedAccountId.Should().Be(changeUserRoleViewModel.HashedAccountId);
        model.TeamMemberActionConfirmation.Success.Should().Be(commandResult.Success);
        model.TeamMemberActionConfirmation.MemberEmail.Should().Be(Uri.UnescapeDataString(changeUserRoleViewModel.Email));
        model.TeamMemberActionConfirmation.TeamMemberAction.Should().Be(TeamMemberAction.ChangeUserRole);
        model.TeamMemberActionConfirmation.Role.Should().Be(changeUserRoleViewModel.Role);
    }
}

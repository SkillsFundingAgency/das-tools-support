using MediatR;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IMediator mediator) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountDetails)]
    public async Task<IActionResult> AccountDetails(string hashedAccountId, AccountFieldsSelection accountFieldsSelection)
    {
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        var result = await mediator.Send(query);

        var viewmodel = AccountDetailsViewModel.MapFrom(result);

        viewmodel.SelectedTab = accountFieldsSelection;

        return View(viewmodel);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_UserOverview)]
    public async Task<IActionResult> UserOverview(Guid userId)
    {
        var query = new GetUserOverviewQuery
        {
            UserId = userId
        };

        var result = await mediator.Send(query);

        var viewmodel = UserOverviewViewModel.MapFrom(result);

        return View(viewmodel);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_InviteTeamMember)]
    public IActionResult InviteTeamMember([FromQuery] string hashedAccountId)
    {
        var viewmodel = new AccountDetailsViewModel
        {
            Account = new Account { HashedAccountId = hashedAccountId },
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
            InvitationViewModel = new InvitationViewModel { HashedAccountId = hashedAccountId }
        };

        return View(RouteNames.EmployerSupport_AccountDetails, viewmodel);
    }

    [HttpPost]
    [Route(RouteNames.EmployerSupport_InviteTeamMember)]
    public async Task<IActionResult> InviteTeamMember(InvitationViewModel invitationModel)
    {
        if (ModelState.IsValid)
        {
            var command = new SendTeamMemberInviteCommand
            {
                HashedAccountId = invitationModel.HashedAccountId,
                FullName = invitationModel.FullName,
                Email = invitationModel.Email,
                Role = invitationModel.Role
            };

            var result = await mediator.Send(command);

            var model = new AccountDetailsViewModel
            {
                Account = new Account { HashedAccountId = invitationModel.HashedAccountId },
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
                HasFormSubmittedSuccessfully = true,
                InvitationViewModel = null,
                TeamMemberActionConfirmation = new TeamMemberActionConfirmation
                {
                    HashedAccountId = invitationModel.HashedAccountId,
                    Success = result.Success,
                    MemberEmail = invitationModel.Email,
                    TeamMemberAction = TeamMemberAction.InviteNewTeamMember
                }
            };

            return View(RouteNames.EmployerSupport_AccountDetails, model);
        }

        return View(RouteNames.EmployerSupport_AccountDetails, new AccountDetailsViewModel
        {
            Account = new Account { HashedAccountId = invitationModel.HashedAccountId },
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
            InvitationViewModel = invitationModel
        });
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_ResendInvitation)]
    public async Task<IActionResult> ResendInvitation([FromQuery] string hashedAccountId, string email)
    {
        if (!string.IsNullOrWhiteSpace(hashedAccountId) && !string.IsNullOrWhiteSpace(email))
        {

            var command = new ResendTeamMemberInvitationCommand
            {
                HashedAccountId = hashedAccountId,
                Email = email
            };

            var result = await mediator.Send(command);

            var model = new AccountDetailsViewModel
            {
                Account = new Account { HashedAccountId = hashedAccountId },
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
                HasFormSubmittedSuccessfully = true,
                InvitationViewModel = null,
                TeamMemberActionConfirmation = new TeamMemberActionConfirmation
                {
                    HashedAccountId = hashedAccountId,
                    Success = result.Success,
                    MemberEmail = Uri.UnescapeDataString(email),
                    TeamMemberAction = TeamMemberAction.ResendInvitation
                }
            };

            return View(RouteNames.EmployerSupport_AccountDetails, model);
        }

        return View(RouteNames.EmployerSupport_AccountDetails, new AccountDetailsViewModel
        {
            Account = new Account { HashedAccountId = hashedAccountId },
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        });
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_ChangeUserRole)]
    public IActionResult ChangUsereRole([FromQuery] string hashedAccountId, Role role, string email, string fullName)
    {
        var viewmodel = new AccountDetailsViewModel
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

        return View(RouteNames.EmployerSupport_AccountDetails, viewmodel);
    }

    [HttpPost]
    [Route(RouteNames.EmployerSupport_ChangeUserRole)]
    public async Task<IActionResult> ChangeUserRole(ChangeUserRoleViewModel changeUserRoleViewModel)
    {
        if (ModelState.IsValid)
        {
            var decodedEmail = Uri.UnescapeDataString(changeUserRoleViewModel.Email);
            var command = new ChangeUserRoleCommand
            {
                HashedAccountId = changeUserRoleViewModel.HashedAccountId,
                Email = decodedEmail,
                Role = changeUserRoleViewModel.Role
            };

            var result = await mediator.Send(command);

            var model = new AccountDetailsViewModel
            {
                Account = new Account { HashedAccountId = changeUserRoleViewModel.HashedAccountId },
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
                HasFormSubmittedSuccessfully = true,
                ChangeUserRoleViewModel = null,
                TeamMemberActionConfirmation = new TeamMemberActionConfirmation
                {
                    HashedAccountId = changeUserRoleViewModel.HashedAccountId,
                    Success = result.Success,
                    MemberEmail = decodedEmail,
                    TeamMemberAction = TeamMemberAction.ChangeUserRole,
                    Role = changeUserRoleViewModel.Role
                }
            };

            return View(RouteNames.EmployerSupport_AccountDetails, model);
        }

        return View(RouteNames.EmployerSupport_AccountDetails, new AccountDetailsViewModel
        {
            Account = new Account { HashedAccountId = changeUserRoleViewModel.HashedAccountId },
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
            ChangeUserRoleViewModel = changeUserRoleViewModel
        });
    }
}
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ChangeUserRole;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.ResendTeamMemberInvitation;
using SFA.DAS.Tools.Support.Infrastructure.Application.Commands.EmployerSupport.SendTeamMemberInvite;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountOrganisations;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetFinanceDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetPayeSchemeLevyDeclarations;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetTeamMembers;
using SFA.DAS.Tools.Support.Infrastructure.Cache;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Account")]
public class AccountDetailsController(IAuthorizationProvider authorizationProvider, IMediator mediator, ICacheService cacheService) : Controller
{
    [HttpGet]
    [Route(RouteNames.Account_Organisations)]
    public async Task<IActionResult> Organsisations(string hashedAccountId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var organisations = await mediator.Send(new GetAccountOrganisationsQuery { HashedAccountId = hashedAccountId });

        var viewModel = new OrganisationViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccount,
            LegalEntities = organisations.LegalEntities
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route(RouteNames.Account_TeamMembers)]
    public async Task<IActionResult> TeamMembers(string hashedAccountId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var teamMemebers = await mediator.Send(new GetTeamMembersQuery { HashedAccountId = hashedAccountId });

        var viewModel = new TeamMembersViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccount,
            TeamMembers = teamMemebers.TeamMembers
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route(RouteNames.Account_Finance)]
    public async Task<IActionResult> Finance(string hashedAccountId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var financeData = await mediator.Send(new GetFinanceDetailsQuery { HashedAccountId = hashedAccountId });

        var viewModel = new FinanceViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccount,
            PayeSchemes = financeData.PayeSchemes,
            Transactions = financeData.Transactions,
            Balance = financeData.Balance
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route(RouteNames.Account_InviteTeamMember)]
    public async Task<IActionResult> InviteTeamMember([FromQuery] string hashedAccountId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var viewmodel = new InvitationViewModel
        {
            HashedAccountId = hashedAccountId,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        return View(viewmodel);
    }

    [HttpPost]
    [Route(RouteNames.Account_InviteTeamMember)]
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

            var viewmodel = new TeamMemberActionConfirmationViewModel
            {
                HashedAccountId = invitationModel.HashedAccountId,
                Success = result.Success,
                MemberEmail = invitationModel.Email,
                TeamMemberAction = TeamMemberAction.InviteNewTeamMember,
                Account = invitationModel.Account,
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam
            };

            return View(viewmodel);
        }

        return View(invitationModel);
    }

    [HttpGet]
    [Route(RouteNames.Account_ResendInvitation)]
    public async Task<IActionResult> ResendInvitation([FromQuery] string hashedAccountId, string email)
    {

        if (!string.IsNullOrWhiteSpace(hashedAccountId) && !string.IsNullOrWhiteSpace(email))
        {
            var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

            var command = new ResendTeamMemberInvitationCommand
            {
                HashedAccountId = hashedAccountId,
                Email = email
            };

            var result = await mediator.Send(command);

            var viewmodel = new TeamMemberActionConfirmationViewModel
            {
                HashedAccountId = hashedAccountId,
                Success = result.Success,
                MemberEmail = Uri.UnescapeDataString(email),
                TeamMemberAction = TeamMemberAction.ResendInvitation,
                Account = accountData,
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam
            };

            return View(viewmodel);
        }

        return View(RouteNames.Account_TeamMembers, new { hashedAccountId });
    }
    
    [HttpGet]
    [Route(RouteNames.Account_PayeSchemeDeclarations)]
    public async Task<IActionResult> PayeSchemeDeclarations([FromQuery] string hashedAccountId, string childId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var query = new GetPayeSchemeLevyDeclarationsQuery
        {
            HashedAccountId = hashedAccountId,
            HashedPayeRef = childId
        };

        var result = await mediator.Send(query);

        var viewmodel = new PayeSchemeLevyDeclarationViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance,
            PayeSchemeName = result.PayeSchemeName,
            PayeSchemeFormatedAddedDate = result.PayeSchemeFormatedAddedDate,
            PayeSchemeRef = result.PayeSchemeRef,
            LevyDeclarations = result.LevyDeclarations,
            UnexpectedError = result.UnexpectedError          
        };


        return View(viewmodel);
    }

    [HttpGet]
    [Route(RouteNames.Account_ChangeUserRole)]
    public async Task<IActionResult> ChangUserRole([FromQuery] string hashedAccountId, Role role, string email, string fullName)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var viewmodel = new ChangeUserRoleViewModel
        {
            HashedAccountId = hashedAccountId,
            Email = email,
            Role = role,
            FullName = fullName,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam
        };

        return View(viewmodel);
    }

    [HttpPost]
    [Route(RouteNames.Account_ChangeUserRole)]
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

            var viewmodel = new TeamMemberActionConfirmationViewModel
            {
                HashedAccountId = changeUserRoleViewModel.HashedAccountId,
                Success = result.Success,
                MemberEmail = decodedEmail,
                TeamMemberAction = TeamMemberAction.ChangeUserRole,
                Role = changeUserRoleViewModel.Role,
                Account = changeUserRoleViewModel.Account,
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam
            };

            return View(RouteNames.Account_TeamMemberActionConfirmation, viewmodel);
        }

        return View(changeUserRoleViewModel);
    }


    private async Task<Account> GetOrSetAccountDetailsInCache(string hashedAccountId)
    {
        var cacheKey = $"AccountDetails_{hashedAccountId}";

        var viewmodel = await cacheService.GetOrSetAsync(cacheKey, () => GetAccountDetailsViewModel(hashedAccountId));

        return viewmodel;
    }

    private async Task<Account> GetAccountDetailsViewModel(string hashedAccountId)
    {
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
        };

        var result = await mediator.Send(query);

        return result.Account;
    }
}

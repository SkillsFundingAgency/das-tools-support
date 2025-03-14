using MediatR;
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
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("accounts")]
public class AccountDetailsController(IAuthorizationProvider authorizationProvider, IMediator mediator, ICacheService cacheService) : AccountBaseController(mediator, cacheService)
{
    [HttpGet]
    [Route("{hashedAccountId}/organisations")]
    public async Task<IActionResult> Organisations(string hashedAccountId)
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
    [Route("{hashedAccountId}/team-members")]
    public async Task<IActionResult> TeamMembers(string hashedAccountId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var teamMemebers = await mediator.Send(new GetTeamMembersQuery { HashedAccountId = hashedAccountId });

        var viewModel = new TeamMembersViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountTeam,
            TeamMembers = teamMemebers.TeamMembers
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route("{hashedAccountId}/finance")]
    public async Task<IActionResult> Finance(string hashedAccountId)
    {
        var isTier1 = await authorizationProvider.IsEmployerSupportTier1Authorized(User);
        if (isTier1)
        {
            var cacheKey = $"FinanceChallenge_{hashedAccountId}_{User.Identity.Name}";

            var formCompleted = await cacheService.RetrieveFromCache<bool>(cacheKey);
            if (!formCompleted)
            {
                return RedirectToAction(RouteNames.Account_Challenge, new { hashedAccountId });
            }
        }

        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var financeData = await mediator.Send(new GetFinanceDetailsQuery { HashedAccountId = hashedAccountId });

        var viewModel = new FinanceViewModel
        {
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance,
            PayeSchemes = financeData.PayeSchemes,
            Transactions = financeData.Transactions,
            Balance = financeData.Balance
        };

        return View(viewModel);
    }

    [HttpGet]
    [Route("{hashedAccountId}/invite-team-member")]
    public async Task<IActionResult> InviteTeamMember(string hashedAccountId)
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
    [Route("{hashedAccountId}/invite-team-member")]
    public async Task<IActionResult> InviteTeamMember(InvitationViewModel invitationModel)
    {
        var accountData = await GetOrSetAccountDetailsInCache(invitationModel.HashedAccountId);

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
                Account = accountData,
                SelectedTab = AccountFieldsSelection.EmployerAccountTeam
            };

            return View("TeamMemberActionConfirmation", viewmodel);
        }

        invitationModel.Account = accountData;
        return View(invitationModel);
    }

    [HttpGet]
    [Route("{hashedAccountId}/resend-invitation")]
    public async Task<IActionResult> ResendInvitation(string hashedAccountId, [FromQuery]string email)
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

            return View("TeamMemberActionConfirmation", viewmodel);
        }

        return View(RouteNames.Account_TeamMembers, new { hashedAccountId });
    }

    [HttpGet]
    [Route("{hashedAccountId}/paye-scheme-declarations")]
    public async Task<IActionResult> PayeSchemeDeclarations(string hashedAccountId, [FromQuery] string childId, [FromQuery] string obscuredPayeRef)
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
            PayeSchemeRef = obscuredPayeRef,
            LevyDeclarations = result.LevyDeclarations,
            UnexpectedError = result.UnexpectedError
        };

        return View(viewmodel);
    }

    [HttpGet]
    [Route("{hashedAccountId}/change-user-role")]
    public async Task<IActionResult> ChangeUserRole(string hashedAccountId, [FromQuery] Role role, [FromQuery] string email, [FromQuery] string fullName)
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
    [Route("{hashedAccountId}/change-user-role")]
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

            return View("TeamMemberActionConfirmation", viewmodel);
        }

        return View(changeUserRoleViewModel);
    }

    [HttpGet]
    [Route("{hashedAccountId}/challenge")]
    public async Task<IActionResult> Challenge(string hashedAccountId)
    {
        var accountData = await GetOrSetAccountDetailsInCache(hashedAccountId);

        var challengeData = await mediator.Send(new GetChallengePermissionQuery { HashedAccountId = hashedAccountId });

        var model = new ChallengeViewModel
        {
            Id = hashedAccountId,
            Characters = challengeData.Characters,
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance
        };
        return View(model);
    }

    [HttpPost]
    [Route("{hashedAccountId}/challenge")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Challenge(ChallengeEntry challengeEntry)
    {
        var accountData = await GetOrSetAccountDetailsInCache(challengeEntry.Id);
        if (ModelState.IsValid)
        {
            var response = await mediator.Send(new ChallengeEntryCommand
            {
                HashedAccountId = challengeEntry.Id,
                Challenge1 = challengeEntry.Challenge1,
                Challenge2 = challengeEntry.Challenge2,
                Balance = challengeEntry.Balance,
                FirstCharacterPosition = challengeEntry.FirstCharacterPosition,
                SecondCharacterPosition = challengeEntry.SecondCharacterPosition
            });

            if (!response.IsValid)
            {
                var viewmodel = new ChallengeViewModel
                {
                    Characters = response.Characters,
                    Id = challengeEntry.Id,
                    HasError = true,
                    Account = accountData,
                    SelectedTab = AccountFieldsSelection.EmployerAccountFinance
                };

                return View(viewmodel);
            }

            var userName = User.Identity.Name;
            var cacheKey = $"FinanceChallenge_{challengeEntry.Id}_{userName}";

            await cacheService.SetAsync(cacheKey, true, 1);

            return RedirectToAction(RouteNames.Account_Finance, new { hashedAccountId = challengeEntry.Id });
        }

        var challengeViewModel = new ChallengeViewModel
        {
            Id = challengeEntry.Id,
            Characters = [challengeEntry.FirstCharacterPosition, challengeEntry.SecondCharacterPosition],
            Account = accountData,
            SelectedTab = AccountFieldsSelection.EmployerAccountFinance,
            HasError = true
        };

        return View(challengeViewModel);
    }
}

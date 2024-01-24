using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("support/user")]
[Authorize(Policy = nameof(PolicyNames.HasTier3Account))]
public class ResumeUserController : UserControllerBase
{
    public ResumeUserController(
        IOptions<ClaimsConfiguration> claimsConfiguration,
        IEmployerUsersService employerUsersService) : base(employerUsersService, claimsConfiguration)
    {
    }

    [HttpPost("ResumeUsers", Name = RouteNames.ResumeUsers)]
    public IActionResult ResumeUsers(UserSearchResultsViewModel model)
    {
        var ids = model.SelectedIds?.Split(',');

        if (ids == null || !ids.Any())
        {
            return RedirectToAction("Index", "SearchUser", new
            {
                HashedAccountId = model.HashedAccountId,
                InternalAccountId = model.InternalAccountId,
                act = ActionNames.Resume
            });
        }

        if (!ResumeUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
        {
            return RedirectToAction("Index", "SearchUser", new
            {
                HashedAccountId = model.HashedAccountId,
                InternalAccountId = model.InternalAccountId,
                act = ActionNames.Resume
            });
        }

        return View("Index", new ResumeUsersViewModel
        {
            Users = users,
            HashedAccountId = model.HashedAccountId,
            InternalAccountId = model.InternalAccountId
        });
    }

    [HttpPost("cancelResumeUsers", Name = RouteNames.CancelResumeUsers)]
    public IActionResult CancelResumeUsers(ResumeUsersViewModel model, string act)
    {
        return RedirectToAction("Index", "SearchUser", new
        {
            HashedAccountId = model.HashedAccountId,
            InternalAccountId = model.InternalAccountId,
            act = ActionNames.Resume
        });
    }

    [HttpPost("resumeUsersConfirmation", Name = RouteNames.ResumeUsersConfirmation)]
    public async Task<IActionResult> ResumeUsersConfirmation(ResumeUsersViewModel model)
    {
        var claims = GetClaims();

        if (!IsValid(model, new string[] { claims.CurrentUserId, claims.CurrentUserEmail }, out IEnumerable<AccountUserRow> users))
        {
            return View(RouteNames.SuspendUsers, model);
        }

        var tasks = users
            .Select(user => EmployerUsersService.ResumeUser(new ResumeUserRequest(user.UserRef, claims.CurrentUserId, claims.CurrentUserEmail), new CancellationToken()));

        var results = await Task.WhenAll(tasks);
        model.Users = CreateUserRows(results, users);
        ModelState.Clear();
        return View("Index", model);
    }

    private bool IsValid(ResumeUsersViewModel model, IEnumerable<string> claims, out IEnumerable<AccountUserRow> userData)
    {
        if (!UsersViewModelBase.TryDeserialise(model.UserData, out userData))
        {
            model.HasError = true;
            model.UserData = null;
            ModelState.AddModelError(string.Empty, "Unable to read user information, please return to the search and try again");

            return false;
        }

        if (!claims.Any(string.IsNullOrWhiteSpace))
        {
            return true;
        }
        
        model.Users = userData;
        ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to Resume User");

        return false;
    }
}
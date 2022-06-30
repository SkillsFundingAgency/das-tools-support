using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/user")]
    public class SuspendUserController : UserControllerBase
    {
        public SuspendUserController(
            ILogger<SuspendUserController> logger,
            IOptions<ClaimsConfiguration> claimConfiguration,
            IEmployerUsersService employerUsersService) : base(employerUsersService, logger, claimConfiguration)
        {
        }

        [HttpPost("suspendUser", Name = RouteNames.SuspendUsers)]
        public IActionResult SuspendUsers(UserSearchResultsViewModel model)
        {
            var ids = model.SelectedIds?.Split(',');

            if (ids == null || !ids.Any())
            {
                return RedirectToAction("Index", "SearchUser", new
                {
                    HashedAccountId = model.HashedAccountId,
                    InternalAccountId = model.InternalAccountId,
                    act = ActionNames.Suspend
                });
            }

            if (!SuspendUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
            {
                return RedirectToAction("Index", "SearchUser", new
                {
                    HashedAccountId = model.HashedAccountId,
                    InternalAccountId = model.InternalAccountId,
                    act = ActionNames.Suspend
                });
            }

            return View("Index", new SuspendUsersViewModel
            {
                Users = users,
                HashedAccountId = model.HashedAccountId,
                InternalAccountId = model.InternalAccountId
            });
        }

        [HttpPost("cancelSuspendUsers", Name = RouteNames.CancelSuspendUsers)]
        public IActionResult CancelSuspendUsers(SuspendUsersViewModel model, string act)
        {
            return RedirectToAction("Index", "SearchUser", new
            {
                HashedAccountId = model.HashedAccountId,
                InternalAccountId = model.InternalAccountId,
                act = ActionNames.Suspend
            });
        }

        [HttpPost("suspendUsersConfirmation", Name = RouteNames.SuspendUsersConfirmation)]
        public async Task<IActionResult> SuspendUsersConfirmation(SuspendUsersViewModel model)
        {
            var claims = GetClaims();

            if (!IsValid(model, new string[] { claims.CurrentUserId, claims.CurrentUserEmail }, out IEnumerable<AccountUserRow> users))
            {
                return View(RouteNames.SuspendUsers, model);
            }

            var tasks = users
                .Where(users => users.ApiSubmissionStatus != SubmissionStatus.Successful)
                .Select(user => _employerUsersService.SuspendUser(new Core.Models.SuspendUserRequest(user.UserRef, claims.CurrentUserId, claims.CurrentUserEmail), new CancellationToken()));

            var results = await Task.WhenAll(tasks);
            model.Users = CreateUserRows(results, users);
            ModelState.Clear();
            return View("Index", model);
        }

        private bool IsValid(SuspendUsersViewModel model, IEnumerable<string> claims, out IEnumerable<AccountUserRow> userData)
        {
            if (!UsersViewModelBase.TryDeserialise(model.UserData, out userData))
            {
                model.HasError = true;
                model.UserData = null;
                ModelState.AddModelError(string.Empty, "Unable to read user information, please return to the search and try again");

                return false;
            }

            if (claims.Any(c => string.IsNullOrWhiteSpace(c)))
            {
                model.Users = userData;
                ModelState.AddModelError(string.Empty, "Unable to retrieve userId or name from claim for request to Suspend User");

                return false;
            }

            return true;
        }
    }
}
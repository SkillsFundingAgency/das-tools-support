using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/user")]
    public class SuspendUserController : UserControllerBase
    {
        private readonly IEmployerUsersService _employerUsersService;

         public SuspendUserController(ILogger<SuspendUserController> logger,
            IEmployerUsersService employerUsersService,
            IMapper mapper)
        {
            _employerUsersService = employerUsersService;
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

            if(!SuspendUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
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
            if(!SuspendUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
            {
                model.HasError = true;
                model.UserData = null;
                ModelState.AddModelError(string.Empty, "Unable to read user information, please return to the search and try again");
                return View("Index", model);
            }

            var tasks = new List<Task<Core.Models.SuspendUserResult>>();

            users.Where(users => users.ApiSubmissionStatus != SubmissionStatus.Successful)
                .ToList()
                .ForEach(user => tasks.Add(_employerUsersService.SuspendUser(new Core.Models.SuspendUserRequest()
            { 
                UserId = user.UserRef 
            }
            , new CancellationToken())));

            var results = await Task.WhenAll(tasks);
            model.Users = CreateUserRows(results, users);
            ModelState.Clear();
            return View("Index", model);
        }
    }
}

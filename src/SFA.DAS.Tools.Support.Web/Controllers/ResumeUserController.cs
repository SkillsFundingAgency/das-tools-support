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
    public class ResumeUserController : UserControllerBase
    {
        private readonly IEmployerUsersService _employerUsersService;

         public ResumeUserController(ILogger<ResumeUserController> logger,
            IEmployerUsersService employerUsersService,
            IMapper mapper)
        {
            _employerUsersService = employerUsersService;
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

            if(!ResumeUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
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
            if(!ResumeUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
            {
                model.HasError = true;
                model.UserData = null;
                ModelState.AddModelError(string.Empty, "Unable to read user information, please return to the search and try again");
                return View("Index", model);
            }

            var tasks = new List<Task<Core.Models.ResumeUserResult>>();

            users.Where(users => users.ApiSubmissionStatus != SubmissionStatus.Successful).ToList().ForEach(user => tasks.Add(_employerUsersService.ResumeUser(new Core.Models.ResumeUserRequest() 
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

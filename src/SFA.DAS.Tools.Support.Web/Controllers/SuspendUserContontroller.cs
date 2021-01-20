using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    [Route("support/user")]
    public class SuspendUserController : Controller
    {
        private readonly ILogger<SuspendUserController> _logger;
        private readonly IEmployerUsersService _employerUsersService;
        private readonly IMapper _mapper;

         public SuspendUserController(ILogger<SuspendUserController> logger,
            IEmployerUsersService employerUsersService,
            IMapper mapper)
        {
            _employerUsersService = employerUsersService;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("suspendUser", Name = RouteNames.SuspendUsers)]
        public IActionResult SuspendUsers(UserSearchResultsViewModel model)
        {
            var ids = model.SelectedIds?.Split(',');

            if (ids == null || ids.Count() == 0)
            {
                return RedirectToAction(RouteNames.EmployerAccountsData_SearchUsers, "SearchUsers", new UserViewModel
                {
                    AccountId = model.AccountId
                });
            }

            if(!SuspendUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
            {
                return RedirectToAction(RouteNames.EmployerAccountsData_SearchUsers, "SearchUsers", new UserViewModel
                {
                    AccountId = model.AccountId
                });
            }
            
            return View("Index", new SuspendUsersViewModel 
            { 
                Users = users,
                AccountId = model.AccountId
            });
        }

        [HttpPost("cancelSuspendUsers", Name = RouteNames.CancelSuspendUsers)]
        public IActionResult CancelSuspendUsers(SuspendUsersViewModel model, string act)
        {
            return RedirectToAction("Index", "UserSearch", new UserSearchResultsViewModel
            {
                AccountId = model.AccountId
            });
        }

        [HttpPost("suspendUsersConfirmation", Name = RouteNames.SuspendUsersConfirmation)]
        public async Task<IActionResult> SuspendUsersConfirmation(SuspendUsersViewModel model)
        {
            if(!SuspendUsersViewModel.TryDeserialise(model.UserData, out IEnumerable<AccountUserRow> users))
            {
                model.HasError = true;
            }

            var result = await _employerUsersService.SuspendUsers(new Core.Models.SuspendUsersRequest
            {
                UserRefs = users.Select(u => u.UserRef)
            }, new CancellationToken());

            foreach (var user in users)
            {
                var r = result.UserRefs.FirstOrDefault(s => s == user.UserRef);
                if (result == null)
                {
                    continue;
                }

                if (!result.HasError)
                {
                    user.ApiSubmissionStatus = SubmissionStatus.Successful;
                    user.ApiErrorMessage = string.Empty;
                }
                else
                {
                    user.ApiSubmissionStatus = SubmissionStatus.Errored;
                    user.ApiErrorMessage = result.ErrorMessage;
                }
            }

            return View("SuspendUser", model);
        }
    }
}

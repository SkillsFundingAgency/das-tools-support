using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public abstract class UserControllerBase : Controller
    {
        protected readonly IEmployerUsersService _employerUsersService;
        protected readonly ILogger _logger;
        private readonly ClaimsConfiguration _claimsConfiguration;
        public UserControllerBase(
            IEmployerUsersService employerUsersService, 
            ILogger logger,
            IOptions<ClaimsConfiguration> claimsConfiguration)
        {
            _employerUsersService = employerUsersService;
            _logger = logger;
            _claimsConfiguration = claimsConfiguration.Value;
        }

       protected IEnumerable<TOut> CreateUserRows<TIn, TOut>(IEnumerable<TIn> results, IEnumerable<TOut> users) 
            where TIn : UserResult
            where TOut : AccountUserRow
        {
            foreach (var user in users)
            {
                var result = results.FirstOrDefault(id => id.UserId == user.UserRef);
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

            return users;
        }

        protected (string CurrentUserId, string CurrentUserEmail) GetClaims()
        {
            return (HttpContext.User.Claims.GetClaim(_claimsConfiguration.EmailClaim), HttpContext.User.Claims.GetClaim(_claimsConfiguration.EmailClaim));
        }
    }
}
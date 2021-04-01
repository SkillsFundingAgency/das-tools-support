using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Tools.Support.Core.Models;
using SFA.DAS.Tools.Support.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public abstract class UserControllerBase : Controller
    {
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
    }
}

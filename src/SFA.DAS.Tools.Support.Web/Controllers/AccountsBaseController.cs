using MediatR;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Cache;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class AccountBaseController(IMediator mediator, ICacheService cacheService) : Controller
{
    protected async Task<Account> GetOrSetAccountDetailsInCache(string hashedAccountId)
    {
        var cacheKey = $"AccountDetails_{hashedAccountId}";

        var viewmodel = await cacheService.GetOrSetAsync(cacheKey, () => GetAccountDetailsViewModel(hashedAccountId));

        return viewmodel;
    }

    protected async Task<Account> GetAccountDetailsViewModel(string hashedAccountId)
    {
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
        };

        var result = await mediator.Send(query);

        return result.Account;
    }
}
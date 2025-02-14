using MediatR;
using SFA.DAS.Tools.Support.Core.Models.EmployerSupport;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IMediator mediator) : Controller
{
    [HttpGet]
    [Route(RouteNames.EmployerSupport_UserSearch)]
    public async Task<IActionResult> EmployerUserSearch(string email)
    {
        var result = await mediator.Send(new GetUsersByEmailQuery {Email = email});

        var model = new EmployerUserSearchModel
        {
            Email = email,
        };

        if (result.Users != null)
        {
            model.Users = result.Users.Select(x => new MatchedUser
                {Id = x.Id.ToString(), DisplayName = x.DisplayName, Email = x.Email}).ToList();
        }

        return View(model);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountSearch)]
    public async Task<IActionResult> EmployerAccountSearch(string publicHashedId, string payeRef)
    {
        var result = await mediator.Send(new GetEmployerAccountsQuery
            {PublicHashedAccountId = publicHashedId, PayeRef = payeRef});
        var model = new EmployerAccountSearchModel {PublicHashedId = publicHashedId, PayeRef = payeRef};
        if (result.Accounts != null)
        {
            model.Accounts = result.Accounts.Select(x => new MatchedAccount
            {
                AccountId = x.AccountId, DasAccountName = x.DasAccountName, HashedAccountId = x.HashedAccountId,
                PublicHashedAccountId = x.PublicHashedAccountId
            }).ToList();
        }
        return View(model);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountDetails)]
    public async Task<IActionResult> AccountDetails(string hashedAccountId, AccountFieldsSelection accountFieldsSelection)
    {
        var query = new GetAccountDetailsQuery
        {
            HashedAccountId = hashedAccountId,
            AccountFieldsSelection = accountFieldsSelection
        };

        var result = await mediator.Send(query);

        var viewmodel = AccountDetailsViewModel.MapFrom(result);

        viewmodel.SelectedTab = accountFieldsSelection;

        return View(viewmodel);
    }
}
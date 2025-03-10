using MediatR;
using SFA.DAS.Encoding;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IMediator mediator, IEncodingService encodingService) : Controller
{

    //public async Task<IActionResult> Index()
    //{
    //    var isEmployerSupportAuthorized = await authorizationProvider.IsEmployerSupportAuthorized(User);

    //    if (!isEmployerSupportAuthorized)
    //    {
    //        return RedirectToAction("Index", "Support");
    //    }

    //    return View();
    //}



    [HttpGet]
    [Route(RouteNames.EmployerSupport_UserSearch)]
    public async Task<IActionResult> EmployerUserSearch(string email)
    {
        var model = new EmployerUserSearchModel
        {
            Email = email,
            SearchMode = SearchMode.UserSearch
        };

        var result = await mediator.Send(new GetUsersByEmailQuery {Email = email});

        if (result.Users != null)
        {
            model.Users = result.Users.Select(x => new MatchedUser
                {Id = x.Id.ToString(), DisplayName = x.DisplayName, Email = x.Email}).ToList();
        }

        return View(model);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_UserOverview)]
    public async Task<IActionResult> UserOverview(Guid userId)
    {
        var query = new GetUserOverviewQuery
        {
            UserId = userId
        };

        var result = await mediator.Send(query);

        var viewmodel = UserOverviewViewModel.MapFrom(result);
        
        return View(viewmodel);
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountSearch)]
    public async Task<IActionResult> EmployerAccountSearch(string publicHashedId, string payeRef)
    {
        var result = await mediator.Send(new GetEmployerAccountsQuery
            {PublicHashedAccountId = publicHashedId, PayeRef = payeRef});
        var model = new EmployerAccountSearchModel
        {
            PublicHashedId = publicHashedId, 
            PayeRef = payeRef, 
            SearchMode = SearchMode.EmployerSearch
        };

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
}
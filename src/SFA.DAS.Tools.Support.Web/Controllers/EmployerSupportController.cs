using MediatR;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetAccountDetails;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IMediator mediator) : Controller
{
    public IActionResult Index()
    {
        return View();
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
}
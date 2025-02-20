using MediatR;
using SFA.DAS.Tools.Support.Core.Models.Enums;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IAuthorizationProvider authorizationProvider, IMediator mediator) : Controller
{
    public async Task<IActionResult> Index()
    {
        var isEmployerSupportAuthorized = await authorizationProvider.IsEmployerSupportAuthorized(User);

        if (!isEmployerSupportAuthorized)
        {
           return RedirectToAction("Index", "Support");
        }

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
}
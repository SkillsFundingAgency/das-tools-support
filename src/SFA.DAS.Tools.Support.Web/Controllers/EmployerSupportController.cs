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
    public IActionResult EmployerUserSearch(string email)
    {
        return View(new EmployerUserSearchModel { Email = email });
    }

    [HttpGet]
    [Route(RouteNames.EmployerSupport_AccountSearch)]
    public IActionResult EmployerAccountSearch(string publicHashedId, string payeRef)
    {
        return View(new EmployerAccountSearchModel { PublicHashedId = publicHashedId, PayeRef = payeRef });
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
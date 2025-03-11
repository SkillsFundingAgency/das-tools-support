using MediatR;
using SFA.DAS.Tools.Support.Infrastructure.Application.Queries.EmployerSupport.GetUserOverview;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models.EmployerSupport;

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
    [Route(RouteNames.EmployerSupport_UserOverview)]
    public async Task<IActionResult> UserOverview(Guid userId)
    {
        var query = new GetUserOverviewQuery
        {
            UserId = userId
        };

        var result = await mediator.Send(query);

        return View((UserOverviewViewModel)result);
    }
}
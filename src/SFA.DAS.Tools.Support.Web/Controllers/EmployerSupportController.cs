using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[Route("Employer")]
public class EmployerSupportController(IAuthorizationProvider authorizationProvider) : Controller
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
}
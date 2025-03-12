using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Models.Error;

namespace SFA.DAS.Tools.Support.Web.Controllers;

[AllowAnonymous]
[Route("/error")]
public class ErrorController : Controller
{
    private readonly IConfiguration _configuration;

    public ErrorController(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [Route("403")]
    public IActionResult AccessDenied()
    {
        return View(new Error403ViewModel(_configuration["ResourceEnvironmentName"]));
    }

    [Route("404")]
    public IActionResult PageNotFound()
    {
        return View();
    }

    [Route("500")]
    public IActionResult ApplicationError()
    {
        return View();
    }
}
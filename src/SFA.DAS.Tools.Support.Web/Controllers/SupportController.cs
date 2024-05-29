using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using SFA.DAS.Tools.Support.Web.Models;

namespace SFA.DAS.Tools.Support.Web.Controllers;

public class SupportController : Controller
{
    private readonly IAuthorizationService _authorizationService;

    public SupportController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<IActionResult> Index()
    {
        var authorizationResult = await _authorizationService.AuthorizeAsync(User, nameof(PolicyNames.HasTier3Account));
        var indexViewModel = new IndexViewModel()
        {
            HasTier3Account = authorizationResult.Succeeded
        };

        return View(indexViewModel);
    }

    [AllowAnonymous]
    public IActionResult LoggedOut()
    {
        return View();
    }
}
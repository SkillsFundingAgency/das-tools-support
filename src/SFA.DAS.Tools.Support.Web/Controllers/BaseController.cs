using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SFA.DAS.Tools.Support.Web.Extensions;
using System;


namespace SFA.DAS.Tools.Support.Web.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {
        private ILogger<T> _logger;
        protected ILogger<T> Logger => _logger ?? (_logger = HttpContext?.RequestServices.GetService<ILogger<T>>());

        public RedirectToActionResult RedirectToAction(string actionName, Type controller, object routeValues)
        {
            return base.RedirectToAction(actionName, controller.GetControllerName(), routeValues);
        }

        public RedirectToActionResult RedirectToAction(string actionName, Type controller)
        {
            return base.RedirectToAction(actionName, controller.GetControllerName());
        }
    }
}

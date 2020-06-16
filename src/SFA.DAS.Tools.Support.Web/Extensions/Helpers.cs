using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.Extensions
{
    public static class Helpers
    {
        public static string GetControllerName(this Type controller)
        {
            return controller.Name.Replace("Controller", "");
        }
    }
}

using System.Security.Claims;

namespace SFA.DAS.Tools.Support.Web.Models;

public class IndexViewModel 
{
    public bool HasTier3Account { get; set; }

    public ClaimsPrincipal User { get; set; }

}
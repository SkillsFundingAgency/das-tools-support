using System.Security.Claims;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetEmailClaim(this ClaimsPrincipal principal, string claimTypeName)
    {
        return principal.Claims.GetClaim(claimTypeName);
    }
}
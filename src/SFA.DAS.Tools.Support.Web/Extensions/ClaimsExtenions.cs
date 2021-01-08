using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SFA.DAS.Tools.Support.Web.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetClaim(this IEnumerable<Claim> claims, string claimTypeName)
        {
            return claims.FirstOrDefault(s => s.Type == claimTypeName)?.Value;
        }
    }
}
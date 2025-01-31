using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Tools.Support.Web.Infrastructure;

public interface IAuthorizationProvider
{
    Task<bool> IsTier1Authorized(ClaimsPrincipal user);
    Task<bool> IsTier2Authorized(ClaimsPrincipal user);
    Task<bool> IsTier3Authorized(ClaimsPrincipal user);
}

public class AuthorizationProvider(IAuthorizationService authorizationService) : IAuthorizationProvider
{
    public async Task<bool> IsTier1Authorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.HasTier1Account));
    }
    
    public async Task<bool> IsTier2Authorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.HasTier2Account));
    }
    
    public async Task<bool> IsTier3Authorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.HasTier3Account));
    }
    
    private async Task<bool> IsAuthorizedFor(ClaimsPrincipal user, string policy)
    {
        var result = await authorizationService.AuthorizeAsync(user, policy);
        return result.Succeeded;
    }
}
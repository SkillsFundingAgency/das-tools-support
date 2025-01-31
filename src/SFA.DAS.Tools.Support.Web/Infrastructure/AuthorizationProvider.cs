using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Tools.Support.Web.Infrastructure;

public interface IAuthorizationProvider
{
    Task<bool> IsEmployerSupportOnlyAuthorized(ClaimsPrincipal user);
    Task<bool> IsSupportAuthorized(ClaimsPrincipal user);
    Task<bool> IsPrivilegeAuthorized(ClaimsPrincipal user);
}

public class AuthorizationProvider(IAuthorizationService authorizationService) : IAuthorizationProvider
{
    public async Task<bool> IsEmployerSupportOnlyAuthorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.EmployerSupportOnly));
    }
    
    public async Task<bool> IsSupportAuthorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.Support));
    }
    
    public async Task<bool> IsPrivilegeAuthorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.Privileged));
    }
    
    private async Task<bool> IsAuthorizedFor(ClaimsPrincipal user, string policy)
    {
        var result = await authorizationService.AuthorizeAsync(user, policy);
        return result.Succeeded;
    }
}
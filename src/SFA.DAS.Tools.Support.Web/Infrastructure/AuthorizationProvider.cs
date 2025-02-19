using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SFA.DAS.Tools.Support.Web.Infrastructure;

public interface IAuthorizationProvider
{
    Task<bool> IsEmployerSupportAuthorized(ClaimsPrincipal user);
    Task<bool> IsEmployerSupportOnlyAuthorized(ClaimsPrincipal user);
    Task<bool> IsEmployerSupportTier1Authorized(ClaimsPrincipal user);
    Task<bool> IsEmployerSupportTier2Authorized(ClaimsPrincipal user);
    Task<bool> IsStopApprenticeshipAuthorized(ClaimsPrincipal user);
    Task<bool> IsPauseOrResumeApprenticeshipAuthorized(ClaimsPrincipal user);
}

public class AuthorizationProvider(IAuthorizationService authorizationService) : IAuthorizationProvider
{
    public async Task<bool> IsEmployerSupportAuthorized(ClaimsPrincipal user)
    {
        return 
            await IsAuthorizedFor(user, nameof(PolicyNames.EmployerSupportTier1))
            ||  await IsAuthorizedFor(user, nameof(PolicyNames.EmployerSupportTier2));
    }
    
    public async Task<bool> IsEmployerSupportOnlyAuthorized(ClaimsPrincipal user)
    {
        var isStopApprenticeshipAuthorized = await IsStopApprenticeshipAuthorized(user);
        var isPauseOrResumeApprenticeshipAuthorized = await IsPauseOrResumeApprenticeshipAuthorized(user);

        return !isStopApprenticeshipAuthorized & !isPauseOrResumeApprenticeshipAuthorized;
    }
    
    public async Task<bool> IsEmployerSupportTier1Authorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.EmployerSupportTier1));
    }
    
    public async Task<bool> IsEmployerSupportTier2Authorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.EmployerSupportTier2));
    }
    
    public async Task<bool> IsStopApprenticeshipAuthorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.StopApprenticeship));
    }
    
    public async Task<bool> IsPauseOrResumeApprenticeshipAuthorized(ClaimsPrincipal user)
    {
        return await IsAuthorizedFor(user, nameof(PolicyNames.PauseOrResumeApprenticeship));
    }
    
    private async Task<bool> IsAuthorizedFor(ClaimsPrincipal user, string policy)
    {
        var result = await authorizationService.AuthorizeAsync(user, policy);
        return result.Succeeded;
    }
}
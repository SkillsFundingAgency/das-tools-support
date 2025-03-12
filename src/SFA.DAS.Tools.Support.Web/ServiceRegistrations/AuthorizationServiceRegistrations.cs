using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class AuthorizationServiceRegistrations
{
    public static void AddAuthorizationService(this IServiceCollection services)
    {
        const string serviceClaimType = "http://service/service";
        
        services
            .AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.EmployerSupportTier1, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCES1);
            })
            .AddPolicy(PolicyNames.EmployerSupportTier2, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCES2);
            })
            .AddPolicy(PolicyNames.StopApprenticeship, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCS);
            })
            .AddPolicy(PolicyNames.PauseOrResumeApprenticeship, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCP);
            });
    }
}
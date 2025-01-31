using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class AuthorizationServiceRegistrations
{
    public static void AddAuthorizationService(this IServiceCollection services)
    {
        const string serviceClaimType = "http://service/service";
        
        services
            .AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.HasTier1Account, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.ESS);
            })
            .AddPolicy(PolicyNames.HasTier2Account, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.ESF);
            })
            .AddPolicy(PolicyNames.HasTier3Account, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCP);
            });
    }
}
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Api.Models;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class AuthorizationServiceRegistrations
{
    public static void AddAuthorizationService(this IServiceCollection services)
    {
        const string serviceClaimType = "http://service/service";
        
        // ESS is allow only in support console
        // SCS is allowed support portal and some bulk stop views
        // SCP is allowed to do everything
        services
            .AddAuthorizationBuilder()
            .AddPolicy(PolicyNames.EmployerSupportOnly, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.ESS);
            })
            .AddPolicy(PolicyNames.Support, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCS);
            })
            .AddPolicy(PolicyNames.Privileged, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(serviceClaimType, UserClaims.SCP);
            });
    }
}
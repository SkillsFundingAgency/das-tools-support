using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.ToolService.Web.AppStart
{
    public static class AuthorizationService
    {
        public static void AddAuthorizationService(this IServiceCollection services)
        {
            const string serviceClaimType = "http://service/service";

            services.AddAuthorization(options =>
                {
                    options.AddPolicy(PolicyNames.HasTier3Account, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim(serviceClaimType, UserClaims.SCP);
                    });
                }
            );
        }
    }
}

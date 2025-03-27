using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.Tools.Support.Web.Infrastructure;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class AuthenticationServiceRegistrations
{
    private const string CookieName = "SFA.DAS.ToolService.Support.Web.Auth";

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureDfESignin(services, configuration);
        return services;
    }

    private static void ConfigureDfESignin(IServiceCollection services, IConfiguration configuration)
    {
        // This ensures the way claims are mapped are consistent with version 7 of OpenIdConnect 
        Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        // register DfeSignIn authentication services to the AspNetCore Authentication Options.
        services.AddAndConfigureDfESignInAuthentication(
            configuration,
            $"{CookieName}",
            typeof(CustomServiceRole),
            DfESignIn.Auth.Enums.ClientName.BulkStop,
            "/signout",
            "");
    }
}
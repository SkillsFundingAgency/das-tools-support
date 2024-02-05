using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.Tools.Support.Web.Configuration;
using SFA.DAS.Tools.Support.Web.Infrastructure;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.Tools.Support.Web.ServiceRegistrations;

public static class AuthenticationServiceRegistrations
{
    private const string CookieName = "SFA.DAS.ToolService.Support.Web.Auth";
    
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var authenticationConfiguration = new AuthenticationConfiguration();
        configuration.GetSection("Authentication").Bind(authenticationConfiguration);

        var useDfESignIn = configuration.GetValue<bool>("UseDfESignIn");

        if (useDfESignIn)
        {
            // register DfeSignIn authentication services to the AspNetCore Authentication Options.
            services.AddAndConfigureDfESignInAuthentication(
                configuration, 
                $"{CookieName}", 
                typeof(CustomServiceRole),
                DfESignIn.Auth.Enums.ClientName.BulkStop,
                "/signout",
                "");
        }
        else
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    options.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                })
                .AddWsFederation(options =>
                {
                    options.Wtrealm = authenticationConfiguration.Wtrealm;
                    options.MetadataAddress = authenticationConfiguration.MetadataAddress;
                    options.UseTokenLifetime = false;
                }).AddCookie(options =>
                {
                    options.AccessDeniedPath = new PathString("/Error/403");
                    options.ExpireTimeSpan = TimeSpan.FromHours(1);
                    options.Cookie.Name = CookieName;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.SlidingExpiration = true;
                    options.Cookie.SameSite = SameSiteMode.None;
                });
        }

        return services;
    }
}
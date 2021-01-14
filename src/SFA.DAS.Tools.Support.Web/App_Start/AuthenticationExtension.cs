using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using SFA.DAS.Tools.Support.Web.Configuration;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var authenticationConfiguration = new AuthenticationConfiguration();
            configuration.GetSection("Authentication").Bind(authenticationConfiguration);

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
                options.Cookie.Name = "SFA.DAS.ToolService.Support.Web.Auth";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
            });

            return services;
        }
    }
}

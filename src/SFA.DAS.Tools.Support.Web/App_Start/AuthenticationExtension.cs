using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(a =>
            {
                a.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                a.DefaultAuthenticateScheme = OpenIdConnectDefaults.AuthenticationScheme;
                a.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://login.microsoftonline.com/1a92889b-8ea1-4a16-8132-347814051567";
                options.ClientId = "";
                options.ClientSecret = "";
                options.ResponseType = OpenIdConnectResponseType.Code;
            }).AddCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = "SFA.DAS.ToolService.Support.Web.Auth";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
            });

            return services;
        }
    }
}

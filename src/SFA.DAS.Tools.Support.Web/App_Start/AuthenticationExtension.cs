﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Tools.Support.Web.App_Start
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LogoutPath = new PathString("/Account/Logout");
                options.AccessDeniedPath = new PathString("/Error/403");
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.Cookie.Name = "SFA.DAS.ToolService.Web.Auth";
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SlidingExpiration = true;
                options.Cookie.SameSite = SameSiteMode.None;
                options.CookieManager = new ChunkingCookieManager() { ChunkSize = 3000 }; options.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = (context) =>
                    {
                        context.HttpContext.Response.Redirect($"https://{configuration["BaseUrl"]}/Account/login?returnUrl=https://localhost:5011/support");
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}

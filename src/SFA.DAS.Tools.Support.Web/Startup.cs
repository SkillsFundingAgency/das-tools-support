using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.IdentityModel.Logging;
using SFA.DAS.Tools.Support.Infrastructure.Services;
using SFA.DAS.Tools.Support.Web.Extensions;
using SFA.DAS.Tools.Support.Web.ServiceRegistrations;

namespace SFA.DAS.Tools.Support.Web;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _env = env;
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CookiePolicyOptions>(options =>
        {
            // This lambda determines whether user consent for non-essential cookies is needed for a given request.
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });
        
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddConfigurationOptions(_configuration);
        services.AddApplicationServices();
        services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        services.AddAuthentication(_configuration);
        services.AddHealthChecks();
        services.AddAuthorizationService();
            
        services.AddRouting(options => options.LowercaseUrls = true);

        services.AddMvc(options =>
        {
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireClaim("http://service/service", _configuration["RequiredRoles"].Split(','))
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
            options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });
        
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.IsEssential = true;
        });

        services.AddDataProtection(_configuration, _env);

        services.AddMediatR(c => c.RegisterServicesFromAssembly(typeof(IEmployerSupportService).Assembly));

        services.AddApplicationInsightsTelemetry();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            IdentityModelEventSource.ShowPII = true;
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedProto
        });

        app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Xss-Protection", "1");
            await next();
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.Use(async (context, next) =>
        {
            if (context.Response.Headers.ContainsKey("X-Frame-Options"))
            {
                context.Response.Headers.Remove("X-Frame-Options");
            }

            context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");

            await next();

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                //Re-execute the request so the user gets the error page
                var originalPath = context.Request.Path.Value;
                context.Items["originalPath"] = originalPath;
                context.Request.Path = "/error/404";
                await next();
            }
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHealthChecks("/health");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                // set the default route based on the UseDfESignIn property from configuration.
                pattern: _configuration.UseDfESignIn() 
                    ? "{controller=Home}/{action=Index}/{id?}" 
                    : "{controller=Support}/{action=Index}/{id?}");
        });
    }
}
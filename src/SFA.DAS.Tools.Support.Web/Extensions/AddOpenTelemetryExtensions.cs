using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Tools.Support.Web.Extensions;

public static class AddOpenTelemetryExtensions
{
    public static string? ResolveApplicationInsightsConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString.Trim();
        }

        connectionString = configuration["ApplicationInsights:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            return connectionString.Trim();
        }

        var instrumentationKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
        if (!string.IsNullOrWhiteSpace(instrumentationKey))
        {
            return $"InstrumentationKey={instrumentationKey.Trim()}";
        }

        return null;
    }

    public static void AddOpenTelemetryRegistration(this IServiceCollection services, string? appInsightsConnectionString)
    {
        if (string.IsNullOrWhiteSpace(appInsightsConnectionString))
        {
            return;
        }

        var connectionString = appInsightsConnectionString.Trim();
        Environment.SetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING", connectionString);

        services.AddOpenTelemetry().UseAzureMonitor(options =>
        {
            options.ConnectionString = connectionString;
        });
    }

    public static IServiceCollection AddOpenTelemetryRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        AddOpenTelemetryRegistration(services, ResolveApplicationInsightsConnectionString(configuration));
        return services;
    }
}

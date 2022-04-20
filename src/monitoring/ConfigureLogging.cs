using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace monitoring;

public static class ConfigureLogging
{
    public static IHostBuilder ConfigureLoggingDefaults(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        var otlpOptions = configuration.GetSection(OtlpOptions.Oltp)
            .Get<OtlpOptions>() ?? new OtlpOptions();
            
        hostBuilder.ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.AddConsole();
            builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
            {
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.IncludeFormattedMessage = true;
            });
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService($"{Assembly.GetExecutingAssembly().GetName().Name ?? "unknown_dotnet"}"));
                options.AddConsoleExporter();
                options.AddOtlpExporter(opt => opt.Endpoint
                    = new Uri(otlpOptions.Endpoint));
            });
        });
        return hostBuilder;
    }
}
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Enrichers.Span;

namespace monitoring;

public static class ConfigureLogging
{
    public static IHostBuilder ConfigureLoggingDefaults(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, builder) =>
        {
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();
            
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
                    .AddService($"{Assembly.GetExecutingAssembly().GetName().Name ?? "unknown_executing_assembly"}"));
                options.AddConsoleExporter();
                options.AddOtlpExporter(opt => opt.Endpoint
                    = new Uri(otlpOptions.Endpoint));
            });
        });

        // hostBuilder.UseSerilog((context, services, config) => config
        //     .ReadFrom.Configuration(context.Configuration)
        //     .ReadFrom.Services(services)
        //     .Enrich.FromLogContext()
        //     .Enrich.WithSpan()
        // );
        return hostBuilder;
    }
}
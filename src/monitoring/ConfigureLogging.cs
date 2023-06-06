using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;

namespace monitoring;

public static class ConfigureLogging
{
    public static IHostBuilder ConfigureDefaultLogging(this IHostBuilder hostBuilder)
    {
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
            
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();
            
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(otlpOptions.ServiceName));
                options.AddConsoleExporter();
                options.AddOtlpExporter(opt => opt.Endpoint
                    = new Uri(otlpOptions.Endpoint));
            });
        });
        return hostBuilder;
    }
    
    public static IHostBuilder ConfigureSerilogLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, config) =>
        {
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();
            
            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                // .Enrich.WithSpan()
                .WriteTo.OpenTelemetry(opts =>
                {
                    opts.Endpoint = otlpOptions.Endpoint;
                    opts.Protocol = OtlpProtocol.Grpc;
                    opts.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = otlpOptions.ServiceName
                    };
                });
        });
        return hostBuilder;
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
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
            
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Otel)
                .Get<OtlpOptions>() ?? new OtlpOptions();
            
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(otlpOptions.ServiceName));
                if (otlpOptions.EnableConsoleExporter)
                {
                    options.AddConsoleExporter();
                }

                options.IncludeFormattedMessage = true;
                options.IncludeScopes = true;
                
                options.AddOtlpExporter((opt, _) =>
                {
                    opt.Endpoint = new Uri($"{otlpOptions.HttpProtobuf}/v1/logs");
                    opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            });
        });
        return hostBuilder;
    }
    
    public static IHostBuilder ConfigureSerilogLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, config) =>
        {
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Otel)
                .Get<OtlpOptions>() ?? new OtlpOptions();

            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithSpan(new SpanOptions
                {
                    IncludeOperationName = true
                })
                .Enrich.WithOpenTelemetryLogEnricher(opts =>
                {
                    opts.ServiceName = otlpOptions.ServiceName;
                })
                .WriteTo.OpenTelemetry(opts =>
                {
                    opts.Endpoint = $"{otlpOptions.HttpProtobuf}/v1/logs";
                    opts.Protocol = OtlpProtocol.HttpProtobuf;
                    opts.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = otlpOptions.ServiceName
                    };
                });
        });
        return hostBuilder;
    }
}
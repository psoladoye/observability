using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;

namespace monitoring;

public static class ConfigureLogging
{
    public static IHostBuilder ConfigureLoggingDefaults(this IHostBuilder hostBuilder, string serviceName)
    {
        hostBuilder.UseSerilog((context, services, config) =>
        {
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();
            
            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .WriteTo.OpenTelemetry(opts =>
                {
                    opts.Endpoint = otlpOptions.Endpoint;
                    opts.Protocol = OtlpProtocol.Grpc;
                    opts.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = serviceName
                    };
                });
        });
        return hostBuilder;
    }
}
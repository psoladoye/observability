using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace monitoring;

public static class ConfigureOpenTelemetry
{
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
        IConfiguration configuration)
    {
        var otlpOptions = configuration.GetSection(OtlpOptions.Oltp)
            .Get<OtlpOptions>() ?? new OtlpOptions();

        services.AddOpenTelemetryTracing((builder) =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(otlpOptions.ServiceName));
                
            builder.AddHttpClientInstrumentation(opt =>
            {
                opt.RecordException = true;
                opt.SetHttpFlavor = true;
                opt.Filter = httpRequestMessage =>
                {
                    string path;
                    try
                    {
                        path = httpRequestMessage!.RequestUri!.PathAndQuery;
                    }
                    catch (InvalidOperationException)
                    {
                        return false;
                    }

                    return !path!.StartsWith("/metrics") && !path.StartsWith("/health");
                };
            });
            builder.AddSource("*");
            builder.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri($"{otlpOptions.HttpProtobuf}/v1/traces");
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        });

        services.AddOpenTelemetryMetrics(builder =>
        {
            builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService(otlpOptions.ServiceName));
            builder.AddHttpClientInstrumentation();
            builder.AddMeter("*");
            builder.AddOtlpExporter(opt =>
            {
                opt.Endpoint = new Uri($"{otlpOptions.HttpProtobuf}/v1/metrics");
                opt.Protocol = OtlpExportProtocol.HttpProtobuf;
            });
        });
        return services;
    }
}
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
        var otlpOptions = configuration.GetSection(OtlpOptions.Otel)
            .Get<OtlpOptions>() ?? new OtlpOptions();

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(otlpOptions.ServiceName))
            .WithTracing(builder =>
            {
                builder.AddSource($"{nameof(IInstrumentation)}.*");
                builder
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                        opts.Filter = httpContext =>
                        {
                            string path;
                            try
                            {
                                path = httpContext.Request.Path;
                            }
                            catch (InvalidOperationException)
                            {
                                return false;
                            }
                        
                            return !path.StartsWith("/metrics") && !path.StartsWith("/health");
                        };
                    })
                    .AddHttpClientInstrumentation(opts =>
                    {
                        opts.RecordException = true;
                        opts.FilterHttpRequestMessage = httpRequestMessage =>
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
                    })
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri($"{otlpOptions.HttpProtobuf}/v1/traces");
                        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                if (otlpOptions.EnableConsoleExporter)
                {
                    builder.AddConsoleExporter();
                }
            })
            .WithMetrics(builder =>
            {
                builder.AddMeter($"{nameof(IInstrumentation)}.*");
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(opt =>
                    {
                        opt.Endpoint = new Uri($"{otlpOptions.HttpProtobuf}/v1/metrics");
                        opt.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
            });
        return services;
    }
}
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace monitoring
{
    public static class ConfigureOpenTelemetry
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services,
            IConfiguration configuration, string service)
        {
            var otlpOptions = configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();

            services.AddOpenTelemetryTracing((builder) =>
            {
                builder.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(service));
                
                builder.AddHttpClientInstrumentation(opt =>
                {
                    opt.RecordException = true;
                    opt.SetHttpFlavor = true;
                    opt.Filter = httpRequestMessage =>
                    {
                        string path;
                        try
                        {
                            path = httpRequestMessage.RequestUri.PathAndQuery;
                        }
                        catch (InvalidOperationException)
                        {
                            return false;
                        }

                        return !path.StartsWith("/metrics") && !path.StartsWith("/health");
                    };
                });
                builder.AddSource("*");
                // builder.AddConsoleExporter();
                builder.AddOtlpExporter(opt => opt.Endpoint 
                    = new Uri(otlpOptions.Endpoint));
            });

            services.AddOpenTelemetryMetrics(builder =>
            {
                builder.AddHttpClientInstrumentation();
                builder.AddMeter("*");
                builder.AddConsoleExporter(options =>
                {
                    // The ConsoleMetricExporter defaults to a manual collect cycle.
                    // This configuration causes metrics to be exported to stdout on a 10s interval.
                    options.MetricReaderType = MetricReaderType.Periodic;
                    options.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
                });
                builder.AddOtlpExporter(opt => opt.Endpoint 
                    = new Uri(otlpOptions.Endpoint));
            });
            return services;
        }
    }
}
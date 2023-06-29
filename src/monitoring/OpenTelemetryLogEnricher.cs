using System.Diagnostics;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace monitoring;

public class OtelEnricherOptions
{
    public string ServiceName { get; set; } = "unknown";
}

public static class OpenTelemetryLogEnricherExtensions
{
    public static LoggerConfiguration WithOpenTelemetryLogEnricher(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration,
        Action<OtelEnricherOptions>? configure)
    {
        ArgumentNullException.ThrowIfNull(loggerEnrichmentConfiguration);

        var options = new OtelEnricherOptions();
        configure?.Invoke(options);
        return loggerEnrichmentConfiguration.With(new OpenTelemetryLogEnricher(options));
    }
}
public class OpenTelemetryLogEnricher : ILogEventEnricher
{
    private readonly string _serviceName;
    
    public OpenTelemetryLogEnricher(OtelEnricherOptions options)
    {
        _serviceName = options.ServiceName;
    }
    
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current == null)
        {
            return;
        }

        logEvent.AddPropertyIfAbsent(new LogEventProperty("service.name",
            new ScalarValue(_serviceName)));
    }
}
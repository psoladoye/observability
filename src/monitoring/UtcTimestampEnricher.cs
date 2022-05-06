using Serilog.Core;
using Serilog.Events;

namespace monitoring;

public class UtcTimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory logEventPropertyFactory)
    {
        var logEventProperty = logEventPropertyFactory.CreateProperty("UtcTimestamp", logEvent.Timestamp.UtcDateTime);
        logEvent.AddPropertyIfAbsent(logEventProperty);
    }
}
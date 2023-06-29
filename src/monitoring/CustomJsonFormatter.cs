using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace monitoring;

public class CustomJsonFormatter : ITextFormatter
{
    private readonly JsonValueFormatter _valueFormatter;
    private readonly string _projectId;

    public CustomJsonFormatter(string projectId, JsonValueFormatter? valueFormatter = null)
    {
        _projectId = projectId;
        _valueFormatter = valueFormatter ?? new JsonValueFormatter(typeTagName: "$type");
    }

    public void Format(LogEvent logEvent, TextWriter output)
    {
        FormatEventToLogEntry(logEvent, output, _valueFormatter);
        output.WriteLine();
    }

    private void FormatEventToLogEntry(LogEvent logEvent, TextWriter output, JsonValueFormatter valueFormatter)
    {
        ArgumentNullException.ThrowIfNull(logEvent);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(valueFormatter);

        try
        {
            var logEntry = new LogEntry
            {
                LogName = $"projects/{_projectId}/logs/{GetString(logEvent.Properties["Application"])}",
                Severity = MapLogLevel(logEvent.Level),
                Timestamp = logEvent.Timestamp.UtcDateTime.ToString("O"),
            };
            
            var properties = new Dictionary<string, object?>();
            foreach (var property in logEvent.Properties)
            {
                if (property.Key.Equals("TraceId", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.Trace = $"projects/{_projectId}/traces/{GetString(logEvent.Properties["TraceId"])}";
                }

                else if (property.Key.Equals("SpanId", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.SpanId = GetString(logEvent.Properties["SpanId"]);
                }

                else if (property.Key.Equals("TraceSampled", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.TraceSampled = GetBoolean(logEvent.Properties["TraceSampled"]);
                }

                else if (property.Key.Equals("SourceContext", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.SourceLocation =
                        new SourceLocation { Function = GetString(logEvent.Properties["SourceContext"]) };
                }

                else if (property.Key.Equals("RequestMethod", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.HttpRequest ??= new HttpRequest();
                    logEntry.HttpRequest.RequestMethod = GetString(logEvent.Properties["RequestMethod"]);
                }
                
                else if (property.Key.Equals("RequestUrl", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.HttpRequest ??= new HttpRequest();
                    logEntry.HttpRequest.RequestUrl = GetString(logEvent.Properties["RequestUrl"]);
                }
                
                else if (property.Key.Equals("RequestScheme", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.HttpRequest ??= new HttpRequest();
                    logEntry.HttpRequest.Protocol = GetString(logEvent.Properties["RequestScheme"]);
                }
                
                else if (property.Key.Equals("RemoteIpAddress", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.HttpRequest ??= new HttpRequest();
                    logEntry.HttpRequest.RemoteIp = GetString(logEvent.Properties["RemoteIpAddress"]);
                }
                
                else if (property.Key.Equals("ServerIpAddress", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.HttpRequest ??= new HttpRequest();
                    logEntry.HttpRequest.ServerIp = GetString(logEvent.Properties["ServerIpAddress"]);
                }
                
                else if (property.Key.Equals("StatusCode", StringComparison.OrdinalIgnoreCase))
                {
                    logEntry.HttpRequest ??= new HttpRequest();
                    logEntry.HttpRequest.Status = GetInt(logEvent.Properties["StatusCode"]);
                }

                else
                {
                    WritePropertyAsDictionary(properties, property.Key, property.Value);
                }
                
            }

            logEntry.Message = logEvent.MessageTemplate.Render(logEvent.Properties);
            logEntry.Properties = properties;
            var jsonString = JsonConvert.SerializeObject(logEntry,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            output.Write(jsonString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void WritePropertyAsDictionary(IDictionary<string, object?> finalStruct, string propKey,
        LogEventPropertyValue propValue)
    {
        switch (propValue)
        {
            case ScalarValue { Value: null }:
                finalStruct.Add(propKey, null);
                break;

            case ScalarValue { Value: bool boolValue }:
                finalStruct.Add(propKey, boolValue);
                break;

            case ScalarValue
            {
                Value: short or ushort or int or uint or long or ulong or float or double or decimal
            } scalarValue:
                // all numbers are converted to double and may lose precision
                // numbers should be sent as strings if they do not fit in a double
                finalStruct.Add(propKey, Convert.ToDouble(scalarValue.Value));
                break;

            case ScalarValue { Value: string stringValue }:
                finalStruct.Add(propKey, stringValue);
                break;

            case ScalarValue scalarValue:
                // handle all other scalar values as strings
                finalStruct.Add(propKey, GetString(scalarValue));
                break;

            case SequenceValue sequenceValue:
                var sequenceChild = new Dictionary<string, object?>();
                for (var i = 0; i < sequenceValue.Elements.Count; i++)
                    WritePropertyAsDictionary(sequenceChild, i.ToString(), sequenceValue.Elements[i]);

                finalStruct.Add(propKey, sequenceChild);
                break;

            case StructureValue structureValue:
                var structureChild = new Dictionary<string, object?>();
                foreach (var childProperty in structureValue.Properties)
                    WritePropertyAsDictionary(structureChild, childProperty.Name, childProperty.Value);

                finalStruct.Add(propKey, structureChild);
                break;

            case DictionaryValue dictionaryValue:
                var dictionaryChild = new Dictionary<string, object?>();
                foreach (var childProperty in dictionaryValue.Elements)
                    WritePropertyAsDictionary(dictionaryChild, childProperty.Key.Value?.ToString() ?? "",
                        childProperty.Value);

                finalStruct.Add(propKey, dictionaryChild);
                break;
        }
    }

    private static string GetString(LogEventPropertyValue v) => (v as ScalarValue)?.Value?.ToString() ?? "";
    private static bool GetBoolean(LogEventPropertyValue v) => (v as ScalarValue)?.Value is true;

    private static int GetInt(LogEventPropertyValue v) => Convert.ToInt32((v as ScalarValue)?.Value);

    private static string MapLogLevel(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose => "DEBUG",
            LogEventLevel.Debug => "DEBUG",
            LogEventLevel.Information => "INFO",
            LogEventLevel.Warning => "WARNING",
            LogEventLevel.Error => "ERROR",
            LogEventLevel.Fatal => "CRITICAL",
            _ => "DEFAULT"
        };
    }
}

internal class SourceLocation
{
    [JsonProperty(PropertyName = "function")]
    public string? Function { get; set; }
}

internal class HttpRequest
{
    [JsonProperty(PropertyName = "requestMethod")]
    public string? RequestMethod { get; set; }
    
    [JsonProperty(PropertyName = "requestUrl")]
    public string? RequestUrl { get; set; }
    
    [JsonProperty(PropertyName = "status")]
    public int? Status { get; set; }
    
    [JsonProperty(PropertyName = "userAgent")]
    public string? UserAgent { get; set; }
    
    [JsonProperty(PropertyName = "severIp")]
    public string? ServerIp { get; set; }
    
    [JsonProperty(PropertyName = "remoteIp")]
    public string? RemoteIp { get; set; }
    
    [JsonProperty(PropertyName = "protocol")]
    public string? Protocol { get; set; }
}

internal class LogEntry
{
    [JsonProperty(PropertyName = "logName")]
    public string LogName { get; set; } = null!;
    
    [JsonProperty(PropertyName = "timestamp")]
    public string Timestamp { get; set; } = null!;
    
    [JsonProperty(PropertyName = "severity")]
    public string Severity { get; set; } = null!;
    
    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; } = null!;
    public Dictionary<string, object?> Properties { get; set; } = null!;
    
    [JsonProperty(PropertyName = "logging.googleapis.com/trace")]
    public string? Trace { get; set; }
    
    [JsonProperty(PropertyName = "logging.googleapis.com/spanId")]
    public string? SpanId { get; set; }
    
    [JsonProperty(PropertyName = "logging.googleapis.com/trace_sampled")]
    public bool? TraceSampled { get; set; }
    
    [JsonProperty(PropertyName = "httpRequest")]
    public HttpRequest? HttpRequest { get; set; }
    
    [JsonProperty(PropertyName = "logging.googleapis.com/sourceLocation")]
    public SourceLocation? SourceLocation { get; set; }
}
namespace monitoring;

public class OtlpOptions
{
    public static readonly string Oltp = "OpenTelemetry";

    public string Endpoint { get; set; } = "http://otel-collector:4317";
}
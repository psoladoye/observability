namespace monitoring;

public class OtlpOptions
{
    public static readonly string Oltp = "OpenTelemetry";

    public string GrpcEndpoint { get; set; } = "http://otel-collector:4317";
    public string HttpProtobuf { get; set; } = "http://otel-collector:4318";
    public string ServiceName { get; set; } = "unknown_service";
}
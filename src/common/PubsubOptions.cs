namespace common;

public class PubsubOptions
{
    public static readonly string Pubsub = nameof(Pubsub);

    public string Hostname { get; set; } = "localhost";
    public bool IsEnabled { get; set; }
}
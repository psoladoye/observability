using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace common;

public interface IRabbitMqConnection
{
    IConnection Connection { get; }
}

public class RabbitMqConnection : IRabbitMqConnection
{
    private const int MaxRetryAttempts = 3;
    private const int RetryDelayMilliseconds = 1000;
    private readonly ILogger<RabbitMqConnection> _logger;

    public RabbitMqConnection(IOptions<PubsubOptions> options, ILogger<RabbitMqConnection> logger)
    {
        _logger = logger;
        var retryPolicy = Policy.Handle<SocketException>()
            .Or<BrokerUnreachableException>()
            .WaitAndRetry(retryCount: 3, attempt => TimeSpan.FromSeconds(5));
        var factory = new ConnectionFactory
        {
            HostName = options.Value.Hostname,
            DispatchConsumersAsync = true
        };
        Connection = retryPolicy.Execute(() =>
        {
            var connection = factory.CreateConnection();
            _logger.LogInformation("[x] RabbitMq connection established");
            return connection;
        });
    }

    public IConnection Connection { get; }
}
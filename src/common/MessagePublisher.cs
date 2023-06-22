using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace common;

public interface IMessagePublisher : IDisposable
{
    Task Send(string message);
}

public class NoopMessagePublisher : IMessagePublisher
{
    public void Dispose()
    {
        
    }

    public Task Send(string message)
    {
        return Task.CompletedTask;
    }
}

public class MessagePublisher : IMessagePublisher
{
    private readonly ILogger<MessagePublisher> _logger;
    private readonly IRabbitMqConnection _rabbitMqConnection;

    public MessagePublisher(ILogger<MessagePublisher> logger, IRabbitMqConnection rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMqConnection = rabbitMqConnection;
    }

    public Task Send(string message)
    {
        using var channel = _rabbitMqConnection.Connection.CreateModel();
        channel.ExchangeDeclare(exchange: "messages", type: ExchangeType.Fanout);
        var body = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(exchange: "messages", routingKey: string.Empty, basicProperties: null, body: body);
        _logger.LogInformation("[x] Sent {Message}", message);
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _rabbitMqConnection.Connection.Dispose();
    }
}
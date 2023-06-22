using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace common;

public interface IMessageListener : IDisposable
{
    void Subscribe();
}

public class NoopMessageListener : IMessageListener
{
    public void Dispose()
    {
        
    }

    public void Subscribe()
    {
        
    }
}

public class MessageListener : IMessageListener
{
    private readonly ILogger<MessageListener> _logger;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private IModel? _consumerChannel;


    public MessageListener(ILogger<MessageListener> logger, IRabbitMqConnection rabbitMqConnection)
    {
        _logger = logger;
        _rabbitMqConnection = rabbitMqConnection;
    }

    private Task Receive(object? model, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        _logger.LogInformation("[x] {Message}", message);
        return Task.CompletedTask;
    }

    public void Subscribe()
    {
        SetChannel();
        StartConsume();
    }

    private void SetChannel()
    {
        _consumerChannel ??= _rabbitMqConnection.Connection.CreateModel();
        _consumerChannel.ExchangeDeclare(exchange: "messages", type: ExchangeType.Fanout);
        _consumerChannel.CallbackException += (sender, args) =>
        {
            _logger.LogWarning(args.Exception, "Recreating RabbitMQ consumer channel");
            _consumerChannel?.Dispose();
            SetChannel();
            StartConsume();
        };
    }

    private void StartConsume()
    {
        var queueName = _consumerChannel.QueueDeclare().QueueName;
        _consumerChannel.QueueBind(queue: queueName, exchange: "messages", routingKey: string.Empty);
        
        var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
        consumer.Received += Receive;
        consumer.Shutdown += (sender, @event) =>
        {
            _logger.LogInformation("[x] Shutdown rabbitmq");
            return Task.CompletedTask;
        };
        consumer.ConsumerCancelled += (sender, @event) =>
        {
            _logger.LogInformation("[x] Cancel rabbitmq");
            return Task.CompletedTask;
        };

        _consumerChannel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);
    }

    public void Dispose()
    {
        _logger.LogInformation("Disposing message listener");
        _consumerChannel?.Dispose();
        _rabbitMqConnection.Connection.Dispose();
    }
}
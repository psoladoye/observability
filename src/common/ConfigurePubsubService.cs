using Microsoft.Extensions.DependencyInjection;

namespace common;

public static class ConfigurePubsubService
{
    public static IServiceCollection AddPubsubPublisher(this IServiceCollection services, bool isEnabled = false)
    {
        if (isEnabled)
        {
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        }
        else
        {
            services.AddSingleton<IMessagePublisher, NoopMessagePublisher>();
        }
        return services;
    }
    
    public static IServiceCollection AddPubsubSubscriber(this IServiceCollection services, bool isEnabled = false)
    {
        if (isEnabled)
        {
            services.AddSingleton<IMessageListener, MessageListener>();
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
        }
        else
        {
            services.AddSingleton<IMessageListener, NoopMessageListener>();
        }
        return services;
    }
}
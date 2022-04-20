namespace web.Metrics;

public static class ConfigureControllerMetrics
{
    public static IServiceCollection AddControllerMetrics(this IServiceCollection services)
    {
        services.AddSingleton<IControllerMetrics, ControllerMetrics>();
        return services;
    }
}
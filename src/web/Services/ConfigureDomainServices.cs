namespace web.Services;

public static class ConfigureDomainServices
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
        return services;
    }
}
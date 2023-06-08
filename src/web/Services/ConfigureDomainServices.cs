namespace web.Services;

public static class ConfigureDomainServices
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddHttpClient<IWeatherForecastService, WeatherForecastService>();
        return services;
    }
}
using System.Diagnostics;
using monitoring;

namespace web.Services;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> Get();
}

internal record Product(int Id, string Title);

public class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ActivitySource _activitySource;

    public WeatherForecastService(ILogger<WeatherForecastService> logger, HttpClient httpClient, IInstrumentation instrumentation)
    {
        _logger = logger;
        _httpClient = httpClient;
        _activitySource = instrumentation.ActivitySource;
    }

    public async Task<WeatherForecast[]> Get()
    {
        var random = new Random();
        _logger.LogInformation("Random number used: {RandomNumber}", random.Next());

        var product = await _httpClient.GetFromJsonAsync<Product>("https://dummyjson.com/products/1");
        
        _logger.LogInformation("Returned product: {Product}", product);

        if (random.Next(0, 3) == 1)
        {
            throw new ApplicationException("Failed to calculate forecast");
        }
        
        var result = CalculateWeatherForecast(random);
        return result;
    }

    private WeatherForecast[] CalculateWeatherForecast(Random random)
    {
        using var activity =
            _activitySource.StartActivity($"{nameof(WeatherForecastService)}.{nameof(CalculateWeatherForecast)}");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = random.Next(-20, 55),
                Summary = Summaries[random.Next(Summaries.Length)]
            })
            .ToArray();
    }
}
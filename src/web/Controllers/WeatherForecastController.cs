using Microsoft.AspNetCore.Mvc;
using web.Metrics;
using web.Services;

namespace web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IControllerMetrics _controllerMetrics;
    private readonly IWeatherForecastService _weatherForecastService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IControllerMetrics controllerMetrics,
        IWeatherForecastService weatherForecastService)
    {
        _logger = logger;
        _controllerMetrics = controllerMetrics;
        _weatherForecastService = weatherForecastService;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.LogInformation("Executing {Method} in Controller", nameof(Get));
        _controllerMetrics.Count();
        return await _weatherForecastService.Get();
    }
}
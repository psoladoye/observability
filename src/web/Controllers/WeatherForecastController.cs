using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using monitoring;
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
    private readonly ActivitySource _activitySource;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IControllerMetrics controllerMetrics,
        IWeatherForecastService weatherForecastService, IInstrumentation instrumentation)
    {
        _logger = logger;
        _controllerMetrics = controllerMetrics;
        _weatherForecastService = weatherForecastService;
        _activitySource = instrumentation.ActivitySource;
    }

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        _logger.LogInformation("Executing {Method} in Controller", nameof(Get));
        _controllerMetrics.Count();
        DoSomeWork();
        return await _weatherForecastService.Get();
    }

    private void DoSomeWork()
    {
        using var activity = _activitySource.StartActivity($"{nameof(WeatherForecastController)}.{nameof(DoSomeWork)}");
        _logger.LogInformation("Doing some work");
        for (var i = 0; i < 200; i++)
        {
            _ = 2 * i;
        }
    }
}
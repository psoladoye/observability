using Microsoft.AspNetCore.Mvc;
using web.Metrics;

namespace web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IControllerMetrics _controllerMetrics;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IControllerMetrics controllerMetrics)
    {
        _logger = logger;
        _controllerMetrics = controllerMetrics;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Executing {Method} in Controller", nameof(Get));
        _controllerMetrics.Count();
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
    }
}
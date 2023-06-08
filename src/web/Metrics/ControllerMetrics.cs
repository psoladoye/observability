
using System.Diagnostics.Metrics;
using monitoring;

namespace web.Metrics;

public class ControllerMetrics : IControllerMetrics
{
    private static readonly Meter Meter = new(
        $"{nameof(IInstrumentation)}.WeatherStation.Forecast", 
        "1.0.0");
    private static readonly Counter<int> Counter = Meter.CreateCounter<int>(
        "total_number_of_get_forecast",
        description: "Count the number of invocations of the controller get method");
        
    public void Count()
    {
        Counter.Add(1); 
    }
}
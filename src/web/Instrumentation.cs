using System.Diagnostics;
using monitoring;

namespace web;

internal class Instrumentation : IInstrumentation
{
    
    public Instrumentation()
    {
        var assemblyName = typeof(Instrumentation).Assembly.GetName();
        var activitySourceName = $"{nameof(IInstrumentation)}.{assemblyName.Name}";
        var version = assemblyName.Version?.ToString();
        ActivitySource = new ActivitySource(activitySourceName ?? "unknown_assembly", version);
    }
    
    public void Dispose()
    {
        ActivitySource.Dispose();
    }

    public ActivitySource ActivitySource { get; }
}
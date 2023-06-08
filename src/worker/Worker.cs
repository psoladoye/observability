using System.Diagnostics;
using System.Reflection;
using monitoring;

namespace worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IWorkerProcessor _workerProcessor;
    private static readonly AssemblyName AssemblyName = typeof(Worker).Assembly.GetName();
    internal static readonly ActivitySource WorkerActivitySource = new(AssemblyName.Name!, AssemblyName.Version!.ToString());
    private readonly ActivitySource _activitySource;

    public Worker(ILogger<Worker> logger, IWorkerProcessor workerProcessor, IInstrumentation instrumentation)
    {
        _logger = logger;
        _workerProcessor = workerProcessor;
        _activitySource = instrumentation.ActivitySource;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (_ = _activitySource.StartActivity($"{nameof(Worker)}.{nameof(ExecuteAsync)}"))
            // using (_ = WorkerActivitySource.StartActivity($"{nameof(Worker)}.{nameof(ExecuteAsync)}"))
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken); 
                await _workerProcessor.Process();
            }
        }
    }
}
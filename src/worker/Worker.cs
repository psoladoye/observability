using System.Diagnostics;
using System.Reflection;

namespace worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IWorkerProcessor _workerProcessor;
    private static readonly AssemblyName AssemblyName = typeof(Worker).Assembly.GetName();
    internal static readonly ActivitySource WorkerActivitySource = new(AssemblyName.Name!, AssemblyName.Version!.ToString());

    public Worker(ILogger<Worker> logger, IWorkerProcessor workerProcessor)
    {
        _logger = logger;
        _workerProcessor = workerProcessor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (_ = WorkerActivitySource.StartActivity($"{nameof(Worker)}.{nameof(ExecuteAsync)}"))
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken); 
                await _workerProcessor.Process();
            }
        }
    }
}
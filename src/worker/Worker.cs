using System.Diagnostics;
using common;
using monitoring;

namespace worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IWorkerProcessor _workerProcessor;
    private readonly ActivitySource _activitySource;
    private readonly IMessageListener _messageListener;

    public Worker(ILogger<Worker> logger, IWorkerProcessor workerProcessor, IInstrumentation instrumentation,
        IMessageListener messageListener)
    {
        _logger = logger;
        _workerProcessor = workerProcessor;
        _messageListener = messageListener;
        _activitySource = instrumentation.ActivitySource;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _messageListener.Subscribe();
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (_ = _activitySource.StartActivity($"{nameof(Worker)}.{nameof(ExecuteAsync)}"))
            {
                _logger.LogInformation("Worker running at: {Time}", DateTimeOffset.Now);
                await Task.Delay(30000, stoppingToken);
                await _workerProcessor.Process();
            }
        }
    }
}
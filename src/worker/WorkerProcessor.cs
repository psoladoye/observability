namespace worker;

public interface IWorkerProcessor
{
    Task Process();
}

public class WorkerProcessor : IWorkerProcessor
{
    private readonly ILogger<WorkerProcessor> _logger;

    public WorkerProcessor(ILogger<WorkerProcessor> logger)
    {
        _logger = logger;
    }

    public Task Process()
    {
        using (_ = Worker.WorkerActivitySource.StartActivity($"{nameof(WorkerProcessor)}.{nameof(Process)}"))
        {
            _logger.LogInformation("Processing data for web service");
            return Task.CompletedTask;   
        }
    }
}
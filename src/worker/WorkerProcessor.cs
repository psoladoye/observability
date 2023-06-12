using System.Diagnostics;
using System.Net.Http.Json;
using monitoring;

namespace worker;

public interface IWorkerProcessor
{
    Task Process();
}

internal record Post(int Id, string Title, int UserId);

public class WorkerProcessor : IWorkerProcessor
{
    private readonly ILogger<WorkerProcessor> _logger;
    private readonly HttpClient _httpClient;
    private readonly ActivitySource _activitySource;

    public WorkerProcessor(ILogger<WorkerProcessor> logger, HttpClient httpClient,
        IInstrumentation instrumentation)
    {
        _logger = logger;
        _httpClient = httpClient;
        _activitySource = instrumentation.ActivitySource;
    }

    public async Task Process()
    {
        using (_ = _activitySource.StartActivity($"{nameof(WorkerProcessor)}.{nameof(Process)}"))
        {
            _logger.LogInformation("Processing data for web service");
            var post = await _httpClient.GetFromJsonAsync<Post>("https://dummyjson.com/post/3");
            _logger.LogInformation("Returned post: {Post}", post);
        }
    }
}
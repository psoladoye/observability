using monitoring;
using Serilog;
using worker;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureSerilogLogging()
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IWorkerProcessor, WorkerProcessor>();
        services.AddOpenTelemetry(hostContext.Configuration);
        services.AddHostedService<Worker>();
    });


try
{
    var app = builder.Build();
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal("Host terminated unexpectedly:  {@Exception}", ex);
}
finally
{
    Log.CloseAndFlush();
}
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
    
    Log.Information("Starting up the application...");
    var assemblyName = typeof(Program).Assembly.GetName();
    Log.Information("Assembly Name: {AssemblyName}", assemblyName.Name);
    Log.Information("Assembly Version: {AssemblyVersion}", assemblyName.Version);
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal("Host terminated unexpectedly:  {@Exception}", ex);
    Console.WriteLine($"Error {ex}");
}
finally
{
    Log.CloseAndFlush();
}
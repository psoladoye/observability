using monitoring;
using Serilog;
using worker;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

const string serviceName = "worker";
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureLoggingDefaults(serviceName)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOpenTelemetry(hostContext.Configuration, serviceName);
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
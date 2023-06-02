using System.Reflection;
using monitoring;
using Serilog;
using Serilog.Events;
using worker;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOpenTelemetry(hostContext.Configuration,
            $"{Assembly.GetExecutingAssembly().GetName().Name ?? "worker"}");
        services.AddHostedService<Worker>();
    })
    .ConfigureLoggingDefaults()
    .UseSerilog();


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
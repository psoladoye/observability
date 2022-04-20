using System.Reflection;
using monitoring;
using worker;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddOpenTelemetry(hostContext.Configuration,
            $"{Assembly.GetExecutingAssembly().GetName().Name ?? "worker"}");
        services.AddHostedService<Worker>();
    })
    .ConfigureLoggingDefaults(new ConfigurationBuilder().Build());


var app = builder.Build();

await app.RunAsync();
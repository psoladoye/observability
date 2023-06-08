using monitoring;
using Serilog;
using worker;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

var builder = Host.CreateDefaultBuilder(args);

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

configurationRoot.Providers.ToList().ForEach(x => Log.Information($"Yes: {x.GetType()}, {x.ToString()}"));
var loggerConfig = configurationRoot.GetSection(LoggerConfigOptions.LoggerConfig)
    .Get<LoggerConfigOptions>() ?? new LoggerConfigOptions();

if (loggerConfig.Use == "serilog")
{
    builder.ConfigureSerilogLogging();
}
else
{
    builder.ConfigureDefaultLogging();
}
builder
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IInstrumentation, Instrumentation>();
        services.AddHttpClient<IWorkerProcessor, WorkerProcessor>();
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
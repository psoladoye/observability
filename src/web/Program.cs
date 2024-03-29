using monitoring;
using Serilog;
using web;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if (string.IsNullOrEmpty(environment))
{
    throw new Exception("Application failed to start. Environment not specified");
}

var configurationRoot = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configurationRoot)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    var loggerConfig = builder.Configuration.GetSection(LoggerConfigOptions.LoggerConfig)
        .Get<LoggerConfigOptions>() ?? new LoggerConfigOptions();

    if (loggerConfig.Use == "serilog")
    {
        builder.Host.ConfigureSerilogLogging();
    }
    else
    {
        builder.Host.ConfigureDefaultLogging();
    }
    
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);
    
    var app = builder.Build();
    
    Log.Information("Starting up the application...");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
    var assemblyName = typeof(Program).Assembly.GetName();
    Log.Information("Assembly Name: {AssemblyName}", assemblyName.Name);
    Log.Information("Assembly Version: {AssemblyVersion}", assemblyName.Version);
    
    startup.Configure(app, app.Environment, loggerConfig);
    
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

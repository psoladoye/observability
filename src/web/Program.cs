using monitoring;
using Serilog;
using web;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.ConfigureSerilogLogging();
    
    var startup = new Startup(builder.Configuration);
    startup.ConfigureServices(builder.Services);
    
    var app = builder.Build();
    
    Log.Information("Starting up the application...");
    Log.Information("Environment: {Environment}", app.Environment.EnvironmentName);
    var assemblyName = typeof(Program).Assembly.GetName();
    Log.Information("Assembly Name: {AssemblyName}", assemblyName.Name);
    Log.Information("Assembly Version: {AssemblyVersion}", assemblyName.Version);
    
    startup.Configure(app, app.Environment);
    
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

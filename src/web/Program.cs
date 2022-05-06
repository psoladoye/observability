using monitoring;
using Serilog;
using web;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.With(new UtcTimestampEnricher())
    )
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.UseStartup<Startup>();
    });

try
{
    var app = builder.Build();
    var hostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
    
    Log.Information("Starting up the application...");
    Log.Information("Environment: {Environment}", hostEnvironment.EnvironmentName);
    var assemblyName = typeof(Program).Assembly.GetName();
    Log.Information("Assembly Name: {AssemblyName}", assemblyName.Name);
    Log.Information("Assembly Version: {AssemblyVersion}", assemblyName.Version);
    
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

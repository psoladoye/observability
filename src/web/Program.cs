using monitoring;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using web;

var expressionTemplate = new ExpressionTemplate("{ {" +
                                                "time: UtcDateTime(@t), " +
                                                // "messageTemplate: @mt, " +
                                                "message: @m, " +
                                                "severity: if @l = 'Information' then 'Info' else @l, " +
                                                "'logging.googleapis.com/sourceLocation': if IsDefined(SourceContext) then {file: SourceContext} else Undefined(), " +
                                                "@x," +
                                                "'logging.googleapis.com/trace': TraceId, " +
                                                "'logging.googleapis.com/spanId': SpanId, " +
                                                "TraceId: Undefined(), " +
                                                "SpanId: Undefined(), " +
                                                "'logging.googleapis.com/labels': Rest(false), " +
                                                "logName: 'projects/paul-soladoye/logs/observability-api'" +
                                                "} }\n",
    theme: TemplateTheme.Code);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureLoggingDefaults("web-api")
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
    
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
    Console.WriteLine($"Error {ex}");
}
finally
{
    Log.CloseAndFlush();
}

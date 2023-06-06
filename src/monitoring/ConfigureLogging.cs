using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Sinks.OpenTelemetry;

namespace monitoring;

class MySerilog
{
    public Dictionary<string, object> Properties { get; set; } = new();
    public string Test1 { get; set; } = "Test1";
    public string Test2 { get; set; } = "Test2";
}

public static class ConfigureLogging
{
    public static IHostBuilder ConfigureDefaultLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.AddConsole();
            
            builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
            {
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.IncludeFormattedMessage = true;
            });
            
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();
            
            builder.AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService($"{Assembly.GetExecutingAssembly().GetName().Name ?? "unknown_executing_assembly"}"));
                options.AddConsoleExporter();
                options.AddOtlpExporter(opt => opt.Endpoint
                    = new Uri(otlpOptions.Endpoint));
            });
        });
        return hostBuilder;
    }
    
    public static IHostBuilder ConfigureSerilogLogging(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, services, config) =>
        {
            var otlpOptions = context.Configuration.GetSection(OtlpOptions.Oltp)
                .Get<OtlpOptions>() ?? new OtlpOptions();

            var mySerilog = context.Configuration.GetSection("Serilog").Get<MySerilog>();
            Console.WriteLine(mySerilog.Test1);
            Console.WriteLine(mySerilog.Test2);
            Console.WriteLine(mySerilog.Properties["Application"]);
            Console.WriteLine(Assembly.GetEntryAssembly().GetName());

            //var functionDependencyContext = DependencyContext.Load(Assembly.GetEntryAssembly());
            
            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithSpan()
                .WriteTo.OpenTelemetry(opts =>
                {
                    opts.Endpoint = otlpOptions.Endpoint;
                    opts.Protocol = OtlpProtocol.Grpc;
                    opts.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = otlpOptions.ServiceName
                    };
                });
        });
        return hostBuilder;
    }
}
using common;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.OpenApi.Models;
using monitoring;
using Serilog;
using web.Metrics;
using web.Services;

namespace web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
        services.AddControllers();
        services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "web", Version = "v1" }); });

        services.AddSingleton<IInstrumentation, Instrumentation>();
        services.AddControllerMetrics();
        services.AddOpenTelemetry(Configuration);
        services.AddDomainServices();

        var pubsubOptions = Configuration.GetSection(PubsubOptions.Pubsub);
        services.Configure<PubsubOptions>(pubsubOptions);
        services.AddPubsubPublisher((pubsubOptions.Get<PubsubOptions>()?? new PubsubOptions()).IsEnabled);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, LoggerConfigOptions loggerConfig)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "web v1");
            c.RoutePrefix = string.Empty;
        });

        if (loggerConfig.Use == "serilog")
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.IncludeQueryInRequestPath = true;
                // Attach additional properties to the request completion event
                opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestUrl", httpContext.Request.GetDisplayUrl());
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                    diagnosticContext.Set("ServerIpAddress", httpContext.Connection.LocalIpAddress);
                };
            });
        }

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthz");
        });
    }
}
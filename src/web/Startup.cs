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
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "web", Version = "v1" });
        });

        services.AddSingleton<IInstrumentation, Instrumentation>();
        services.AddControllerMetrics();
        services.AddOpenTelemetry(Configuration);
        services.AddDomainServices();
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
            app.UseSerilogRequestLogging();
        }
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}
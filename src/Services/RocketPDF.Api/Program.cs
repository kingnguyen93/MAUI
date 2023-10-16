using Hangfire;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using RocketPDF.Api.Configurations;
using RocketPDF.Application;
using RocketPDF.EntityFrameworkCore;
using RocketPDF.Hangfire;
using RocketPDF.Infrastructure;
using RocketPDF.Infrastructure.Common;
using RocketPDF.Infrastructure.Extensions;
using RocketPDF.Infrastructure.Middlewares;
using RocketPDF.Infrastructure.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Grafana.Loki;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services
    .AddEntityFrameworkCore(builder.Configuration.GetConnectionString("DataConnection"));

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    });

builder.Services
    .AddOptions()
    .AddHttpClient()
    .AddHttpContextAccessor()
    .ResolveApplicationDependencies()
    .ResolveInfrastructureDependencies()
    .ConfigureCache(builder.Configuration)
    .ConfigureHealthChecks(builder.Configuration)
    .ConfigureJwtAuthentication(builder.Configuration)
    .ConfigureHangfire(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // options.SuppressModelStateInvalidFilter = true;
    options.InvalidModelStateResponseFactory = context =>
    {
        return new BadRequestObjectResult(ApiResponseWrapper.BadRequest(false, message: context.ModelState.GetStringError(), errors: context.ModelState
            .Where(error => error.Value?.Errors.Count > 0)
            .Select(error => new
            {
                Field = error.Key,
                Description = error.Value?.Errors.FirstOrDefault()?.ErrorMessage
            })));
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

#if DEBUG
builder.Host.UseSerilog((_, cfg) => cfg.WriteTo.Console());
#else
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg
        .MinimumLevel.Warning()
        .MinimumLevel.Override("RocketPDF", LogEventLevel.Debug)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "rocket-pdf-api")
        .WriteTo.GrafanaLoki(ctx.Configuration.GetValue<string>("Loki:Url")!);
});
#endif

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseRouting();

// Use an exception handler middleware before any other handlers
app.UseCustomeExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// GlobalJobFilters.Filters.Remove(GlobalJobFilters.Filters.Single(x => x.Instance is CaptureCultureAttribute).Instance);
// Configure hangfire to use the new JobActivator we defined.
GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(app.Services));

app.MapControllers().RequireAuthorization();

app.MapHealthChecks(HealthCheck.EndPoint, new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks(HealthCheck.Self, new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.MapHangfireDashboard("/hangfire", new DashboardOptions
{
    AsyncAuthorization = new[]
    {
        new  HangfireAuthorizationFilter()
    }
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = HealthCheck.UI;
});

app.Run();

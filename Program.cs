using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Shinra.Config;
using System;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
Log.Information("Starting Up");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.ClearProviders();
    builder.Logging.AddSerilog();
    builder.Services.AddServices();
    builder.Host.UseSerilog((hostBuilder, services, logger) => logger.BuildLoggerConfig(builder.Configuration));
    var app = builder.Build();
    app.Configure();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Unhandled Exception");
}
finally
{
    Log.Information("Shutdown Complete");
    Log.CloseAndFlush();
}
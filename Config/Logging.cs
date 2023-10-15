using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using System;

namespace Shinra.Config
{
    internal static class Logging
    {
        internal static LoggingLevelSwitch _loglevel = new(LogEventLevel.Warning);
        internal static LoggerConfiguration BuildLoggerConfig(this LoggerConfiguration logger, IConfiguration configuration)
        {
            var mongodbconfig = configuration["MongoConnectionString"];
            Serilog.Debugging.SelfLog.Enable(Console.Error);
            logger = logger
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .MinimumLevel.ControlledBy(_loglevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

            logger.WriteTo.MongoDBBson($"{configuration["MongoConnectionString"]}/reach", "log", period: TimeSpan.FromSeconds(1), cappedMaxSizeMb: 500, cappedMaxDocuments: 2000);

            return logger;
        }
    }
}

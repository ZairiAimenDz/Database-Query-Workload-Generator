using Serilog;
using Serilog.Events;

namespace DatabaseWorkloadQueryGenerator
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddSerilogConfiguration(this ILoggingBuilder logger)
        {
            logger.ClearProviders();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("*.Controllers", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] [Done in {ElapsedMs}ms] {SourceContext}: {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.File(
                    "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] [Done in {ElapsedMs}ms] {RequestId} {RequestPath} {HttpMethod} | {Message:lj}{NewLine}{Exception}"
                )
                .WriteTo.Seq(
                    serverUrl: "https://seq.zairiaimen.com/ingest",
                    apiKey: "jOQhU34PjFXV4tQoDAGL",
                    restrictedToMinimumLevel: LogEventLevel.Warning,
                    batchPostingLimit: 100,
                    period: TimeSpan.FromSeconds(5)
                )
                .Filter.ByExcluding(e =>
                    e.Properties.ContainsKey("SourceContext") &&
                    (e.Properties["SourceContext"].ToString().Contains("EntityFramework.Database") ||
                     e.Properties["SourceContext"].ToString().Contains("EntityFramework.Migrations")) &&
                    e.Level < LogEventLevel.Error)
                .CreateLogger();
            return logger.AddSerilog();
        }
    }
}

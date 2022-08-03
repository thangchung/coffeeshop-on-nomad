using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace N8T.Infrastructure.OTel;

public static class Extensions
{
    public static IWebHostBuilder AddOTelLogs(this IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, builder) => {
            builder.ClearProviders();
            builder.AddConsole();

            var logExporter = context.Configuration.GetValue<string>("UseLogExporter").ToLowerInvariant();
            switch (logExporter)
            {
                case "console":
                    builder.AddOpenTelemetry(options =>
                    {
                        options.AddConsoleExporter();
                    });
                    break;
                default:
                    break;
            }

            builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
            {
                opt.IncludeScopes = true;
                opt.ParseStateValues = true;
                opt.IncludeFormattedMessage = true;
            });
        });
        
        return hostBuilder;
    }

    public static IServiceCollection AddOTelTracing(this IServiceCollection services, IConfiguration config)
    {
        var tracingExporter = config.GetValue<string>("UseTracingExporter").ToLowerInvariant();
        
        switch (tracingExporter)
        {
            case "console":
                services.AddOpenTelemetryTracing((builder) => builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMassTransitInstrumentation()
                    .AddConsoleExporter());
                // For options which can be bound from IConfiguration.
                services.Configure<AspNetCoreInstrumentationOptions>(config.GetSection("AspNetCoreInstrumentation"));
                // For options which can be configured from code only.
                services.Configure<AspNetCoreInstrumentationOptions>(options =>
                {
                    options.Filter = (req) =>
                    {
                        return req.Request?.Host != null;
                    };
                });
                break;
            default:
                break;
        }
        
        return services;
    }

    public static IServiceCollection AddOTelMetrics(this IServiceCollection services, IConfiguration config)
    {
        var metricsExporter = config.GetValue<string>("UseMetricsExporter").ToLowerInvariant();

        services.AddOpenTelemetryMetrics(bd =>
        {
            bd.AddAspNetCoreInstrumentation();
            bd.AddHttpClientInstrumentation();

            switch (metricsExporter)
            {
                case "console":
                    bd.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
                    {
                        exporterOptions.Targets = ConsoleExporterOutputTargets.Console;

                        // The ConsoleMetricExporter defaults to a manual collect cycle.
                        // This configuration causes metrics to be exported to stdout on a 10s interval.
                        // metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
                        metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
                    });
                    break;
                default:
                    break;
            }
        });

        return services;
    }
}

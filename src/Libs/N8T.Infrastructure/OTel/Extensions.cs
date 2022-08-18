using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace N8T.Infrastructure.OTel;

public static class Extensions
{
    public static IWebHostBuilder AddOTelLogs(this IWebHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.AddConsole();

            var logExporter = context.Configuration.GetValue<string>("UseLogExporter").ToLowerInvariant();
            switch (logExporter)
            {
                case "console":
                    builder.AddOpenTelemetry(options => { options.AddConsoleExporter(); });
                    break;
                case "otlp":
                    builder.AddOpenTelemetry(options =>
                    {
                        options.SetResourceBuilder(ResourceBuilder.CreateDefault()
                            .AddService(context.Configuration.GetValue<string>("Otlp:ServiceName")));
                        options.AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new Uri(context.Configuration.GetValue<string>("Otlp:Endpoint"));
                        });
                    });
                    break;
                case "none":
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
                    options.Filter = req => req.Request.Host != null;
                });
                break;
            case "otlp":
                services.AddOpenTelemetryTracing((builder) => builder
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(config.GetValue<string>("Otlp:ServiceName")))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(config.GetValue<string>("Otlp:Endpoint"));
                    }));
                break;
            case "none":
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
                case "otlp":
                    bd.AddOtlpExporter();
                    break;
                case "none":
                    break;
            }
        });

        return services;
    }
}

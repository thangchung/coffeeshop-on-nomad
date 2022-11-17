using System;
using Microsoft.AspNetCore.Builder;
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
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder.CreateDefault()
                        .AddService(builder.Configuration.GetValue<string>("Otlp:ServiceName"));

        Console.WriteLine($"Otlp:ServiceName: {builder.Configuration.GetValue<string>("Otlp:ServiceName")}");
        Console.WriteLine($"Otlp:Endpoint: {builder.Configuration.GetValue<string>("Otlp:Endpoint")}");

        var logExporter = builder.Configuration.GetValue<string>("UseLogExporter").ToLowerInvariant();
        builder.Logging.AddOpenTelemetry(o =>
        {
            // TODO: Setup an exporter here
            o.SetResourceBuilder(resourceBuilder);
            switch (logExporter)
            {
                case "console":
                    o.AddConsoleExporter();
                    break;
                case "otlp":
                    o.SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(builder.Configuration.GetValue<string>("Otlp:ServiceName")));
                    o.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"));
                    });
                    break;
                case "":
                case "none":
                    break;
            }
        });


        // switch (logExporter)
        // {
        //     case "console":
        //         builder.AddOpenTelemetry(options => { options.AddConsoleExporter(); });
        //         break;
        //     case "otlp":
        //         builder.AddOpenTelemetry(options =>
        //         {
        //             options.SetResourceBuilder(ResourceBuilder.CreateDefault()
        //                 .AddService(context.Configuration.GetValue<string>("Otlp:ServiceName")));
        //             options.AddOtlpExporter(otlpOptions =>
        //             {
        //                 otlpOptions.Endpoint = new Uri(context.Configuration.GetValue<string>("Otlp:Endpoint"));
        //             });
        //         });
        //         break;
        //     case "":
        //     case "none":
        //         break;
        // }

        // builder.Services.AddOpenTelemetryMetrics(metrics =>
        // {
        //     metrics.SetResourceBuilder(resourceBuilder)
        //            .AddPrometheusExporter()
        //            .AddAspNetCoreInstrumentation()
        //            .AddRuntimeInstrumentation()
        //            .AddHttpClientInstrumentation()
        //            .AddEventCountersInstrumentation(c =>
        //            {
        //                // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
        //                c.AddEventSources(
        //                    "Microsoft.AspNetCore.Hosting",
        //                    // There's currently a bug preventing this from working
        //                    // "Microsoft-AspNetCore-Server-Kestrel"
        //                    "System.Net.Http", 
        //                    "System.Net.Sockets",
        //                    "System.Net.NameResolution",
        //                    "System.Net.Security");
        //            });
        // });

        var metricsExporter = builder.Configuration.GetValue<string>("UseMetricsExporter").ToLowerInvariant();

        // void ConfigureResource(ResourceBuilder r) => r.AddService(config.GetValue<string>("Otlp:ServiceName"),
        //     serviceVersion: "unknown", serviceInstanceId: Environment.MachineName);

        builder.Services.AddOpenTelemetryMetrics(bd =>
        {
            bd.SetResourceBuilder(resourceBuilder)
                //.ConfigureResource(ConfigureResource)
                .AddPrometheusExporter()
                .AddRuntimeInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEventCountersInstrumentation(c =>
                   {
                       // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                       c.AddEventSources(
                           "Microsoft.AspNetCore.Hosting",
                           // There's currently a bug preventing this from working
                           // "Microsoft-AspNetCore-Server-Kestrel"
                           "System.Net.Http",
                           "System.Net.Sockets",
                           "System.Net.NameResolution",
                           "System.Net.Security");
                   });

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
                    bd.AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"));
                    });
                    break;
                case "":
                case "none":
                    break;
            }
        });

        // builder.Services.AddOpenTelemetryTracing(tracing =>
        // {
        //     // TODO: Setup an exporter here
        //     tracing.SetResourceBuilder(resourceBuilder)
        //            .AddAspNetCoreInstrumentation()
        //            .AddHttpClientInstrumentation()
        //            .AddEntityFrameworkCoreInstrumentation();

        // });

        var tracingExporter = builder.Configuration.GetValue<string>("UseTracingExporter").ToLowerInvariant();

        switch (tracingExporter)
        {
            case "console":
                builder.Services.AddOpenTelemetryTracing(b => b
                    .AddSource("MassTransit") // https://github.com/open-telemetry/opentelemetry-dotnet-contrib/issues/326
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddConsoleExporter());
                // For options which can be bound from IConfiguration.
                builder.Services.Configure<AspNetCoreInstrumentationOptions>(builder.Configuration.GetSection("AspNetCoreInstrumentation"));
                // For options which can be configured from code only.
                builder.Services.Configure<AspNetCoreInstrumentationOptions>(options =>
                {
                    options.Filter = _ => true;
                });
                break;
            case "otlp":
                builder.Services.AddOpenTelemetryTracing(b => b
                    .AddSource("MassTransit") // https://github.com/open-telemetry/opentelemetry-dotnet-contrib/issues/326
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation(b => b.SetDbStatementForText = true)
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Endpoint = new Uri(builder.Configuration.GetValue<string>("Otlp:Endpoint"));
                    }));
                break;
            case "":
            case "none":
                break;
        }

        return builder;
    }

    // public static IWebHostBuilder AddOTelLogs(this IWebHostBuilder hostBuilder)
    // {
    //     hostBuilder.ConfigureLogging((context, builder) =>
    //     {
    //         builder.ClearProviders();
    //         builder.AddConsole();

    //         var logExporter = context.Configuration.GetValue<string>("UseLogExporter").ToLowerInvariant();
    //         switch (logExporter)
    //         {
    //             case "console":
    //                 builder.AddOpenTelemetry(options => { options.AddConsoleExporter(); });
    //                 break;
    //             case "otlp":
    //                 builder.AddOpenTelemetry(options =>
    //                 {
    //                     options.SetResourceBuilder(ResourceBuilder.CreateDefault()
    //                         .AddService(context.Configuration.GetValue<string>("Otlp:ServiceName")));
    //                     options.AddOtlpExporter(otlpOptions =>
    //                     {
    //                         otlpOptions.Endpoint = new Uri(context.Configuration.GetValue<string>("Otlp:Endpoint"));
    //                     });
    //                 });
    //                 break;
    //             case "":
    //             case "none":
    //                 break;
    //         }

    //         builder.Services.Configure<OpenTelemetryLoggerOptions>(opt =>
    //         {
    //             opt.IncludeScopes = true;
    //             opt.ParseStateValues = true;
    //             opt.IncludeFormattedMessage = true;
    //         });
    //     });

    //     return hostBuilder;
    // }

    // public static IServiceCollection AddOTelTracing(this IServiceCollection services, IConfiguration config)
    // {
    //     var tracingExporter = config.GetValue<string>("UseTracingExporter").ToLowerInvariant();

    //     switch (tracingExporter)
    //     {
    //         case "console":
    //             services.AddOpenTelemetryTracing(builder => builder
    //                 .AddSource("MassTransit") // https://github.com/open-telemetry/opentelemetry-dotnet-contrib/issues/326
    //                 .AddAspNetCoreInstrumentation()
    //                 .AddHttpClientInstrumentation()
    //                 .AddEntityFrameworkCoreInstrumentation()
    //                 .AddConsoleExporter());
    //             // For options which can be bound from IConfiguration.
    //             services.Configure<AspNetCoreInstrumentationOptions>(config.GetSection("AspNetCoreInstrumentation"));
    //             // For options which can be configured from code only.
    //             services.Configure<AspNetCoreInstrumentationOptions>(options =>
    //             {
    //                 options.Filter = _ => true;
    //             });
    //             break;
    //         case "otlp":
    //             services.AddOpenTelemetryTracing(builder => builder
    //                 .AddSource("MassTransit") // https://github.com/open-telemetry/opentelemetry-dotnet-contrib/issues/326
    //                 .SetResourceBuilder(ResourceBuilder.CreateDefault()
    //                     .AddService(config.GetValue<string>("Otlp:ServiceName")))
    //                 .AddAspNetCoreInstrumentation()
    //                 .AddHttpClientInstrumentation()
    //                 .AddEntityFrameworkCoreInstrumentation(b => b.SetDbStatementForText = true)
    //                 .AddOtlpExporter(otlpOptions =>
    //                 {
    //                     otlpOptions.Endpoint = new Uri(config.GetValue<string>("Otlp:Endpoint"));
    //                 }));
    //             break;
    //         case "":
    //         case "none":
    //             break;
    //     }

    //     return services;
    // }

    // public static IServiceCollection AddOTelMetrics(this IServiceCollection services, IConfiguration config)
    // {
    //     var metricsExporter = config.GetValue<string>("UseMetricsExporter").ToLowerInvariant();

    //     void ConfigureResource(ResourceBuilder r) => r.AddService(config.GetValue<string>("Otlp:ServiceName"),
    //         serviceVersion: "unknown", serviceInstanceId: Environment.MachineName);

    //     services.AddOpenTelemetryMetrics(bd =>
    //     {
    //         bd.ConfigureResource(ConfigureResource)
    //             .AddRuntimeInstrumentation()
    //             .AddAspNetCoreInstrumentation()
    //             .AddHttpClientInstrumentation();

    //         switch (metricsExporter)
    //         {
    //             case "console":
    //                 bd.AddConsoleExporter((exporterOptions, metricReaderOptions) =>
    //                 {
    //                     exporterOptions.Targets = ConsoleExporterOutputTargets.Console;

    //                     // The ConsoleMetricExporter defaults to a manual collect cycle.
    //                     // This configuration causes metrics to be exported to stdout on a 10s interval.
    //                     // metricReaderOptions.MetricReaderType = MetricReaderType.Periodic;
    //                     metricReaderOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
    //                 });
    //                 break;
    //             case "otlp":
    //                 bd.AddOtlpExporter(otlpOptions =>
    //                 {
    //                     otlpOptions.Endpoint = new Uri(config.GetValue<string>("Otlp:Endpoint"));
    //                 });
    //                 break;
    //             case "":
    //             case "none":
    //                 break;
    //         }
    //     });

    //     return services;
    // }
}
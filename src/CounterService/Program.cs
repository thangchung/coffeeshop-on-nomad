// dotnet ef migrations add InitCounterDb -c MainDbContext -o Infrastructure/Data/Migrations

using CounterService.Consumers;
using CounterService.Domain;
using CounterService.Features;
using CounterService.Infrastructure.Data;
using CounterService.Infrastructure.Gateways;
using CounterService.Infrastructure.Hubs;
using MassTransit;
using N8T.Infrastructure;
using N8T.Infrastructure.Controller;
using N8T.Infrastructure.EfCore;
using N8T.Infrastructure.OTel;
using Spectre.Console;
using System.Net;

AnsiConsole.Write(new FigletText("Counter APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);

builder.WebHost
    // .AddOTelLogs()
    .ConfigureKestrel(webBuilder =>
    {
        webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 5002)); // REST
    });

builder.Services
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] {typeof(Order)})
    .AddCustomValidators(new[] {typeof(Order)});

builder.Services
    .AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("counterdb"),
        null,
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSignalR();

// builder.Services
//     .AddOTelTracing(builder.Configuration)
//     .AddOTelMetrics(builder.Configuration);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BaristaOrderUpdatedConsumer>(typeof(BaristaOrderUpdatedConsumerDefinition));
    x.AddConsumer<KitchenOrderUpdatedConsumer>(typeof(KitchenOrderUpdatedConsumerDefinition));

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMqUrl")!);
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddHttpClient();
builder.Services.AddScoped<IItemGateway, ItemRestGateway>();

builder.AddOpenTelemetry();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
    .ExcludeFromDescription();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

// var orderGroup = app.MapGroup("/v1/api/orders");

_ = app.MapOrderInApiRoutes()
    .MapOrderFulfillmentApiRoutes();

app.MapHub<NotificationHub>("/message");

await app.DoDbMigrationAsync(app.Logger);

// Configure the prometheus endpoint for scraping metrics
app.MapPrometheusScrapingEndpoint();
// NOTE: This should only be exposed on an internal port!
// .RequireHost("*:9100");

app.Run();
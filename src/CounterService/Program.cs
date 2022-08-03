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

AnsiConsole.Write(new FigletText("Counter APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs();

builder.Services
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] { typeof(Order) })
    .AddCustomValidators(new[] { typeof(Order) });

builder.Services
    .AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("counterdb"),
        null,
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddSignalR();

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

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

builder.Services.AddGrpcClient<CoffeeShop.Protobuf.Item.V1.ItemApi.ItemApiClient>("ItemClient", o => {
    o.Address = new Uri(builder.Configuration.GetValue<string>("ProductUri")!);
});

builder.Services.AddScoped<IItemGateway, ItemGateway>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.MapGet("/error", () => Results.Problem("An error occurred.", statusCode: 500))
    .ExcludeFromDescription();

app.UseMiddleware<ExceptionMiddleware>();

app.UseRouting();

//app.UseAuthorization();

app.MapOrderInApiRoutes();
app.MapOrderFulfillmentApiRoutes();

app.MapHub<NotificationHub>("/message");

await app.DoDbMigrationAsync(app.Logger);

app.Run();

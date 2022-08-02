// dotnet ef migrations add InitCounterDb -c MainDbContext -o Infrastructure/Data/Migrations

using CoffeeShop.Counter.Features;
using CoffeeShop.Infrastructure.Data;
using CoffeeShop.Infrastructure.Hubs;
using CounterService.Consumers;
using CounterService.Domain;
using CounterService.Infrastructure.Gateways;
using MassTransit;
using N8T.Infrastructure;
using N8T.Infrastructure.Controller;
using N8T.Infrastructure.EfCore;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Counter APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCors(options =>
    {
        options.AddPolicy(
            name: "api",
            builder =>
            {
                builder
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials();
            });
    })
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] { typeof(Order) })
    .AddCustomValidators(new[] { typeof(Order) });

builder.Services
    .AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("counterdb"),
        null,
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BaristaOrderUpdatedConsumer>(typeof(BaristaOrderUpdatedConsumerDefinition));
    x.AddConsumer<KitchenOrderUpdatedConsumer>(typeof(KitchenOrderUpdatedConsumerDefinition));

    x.SetKebabCaseEndpointNameFormatter();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddGrpcClient<CoffeeShop.Protobuf.Item.V1.ItemApi.ItemApiClient>("ItemClient", o => {
    o.Address = new Uri("http://localhost:15001");
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

app.UseCors("api");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthorization();

app.MapOrderInApiRoutes();
app.MapOrderFulfillmentApiRoutes();

app.MapHub<NotificationHub>("/message");

app.MapFallback(() => Results.Redirect("/swagger"));

await app.DoDbMigrationAsync(app.Logger);

app.Run();

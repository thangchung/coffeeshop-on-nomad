// dotnet ef migrations add InitKitchenDb -c MainDbContext -o Infrastructure/Data/Migrations
    
using CoffeeShop.Infrastructure.Data;
using KitchenService.Consumers;
using KitchenService.Domain;
using KitchenService.Infrastructure.Data;
using MassTransit;
using N8T.Infrastructure;
using N8T.Infrastructure.Controller;
using N8T.Infrastructure.EfCore;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Kitchen APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] { typeof(KitchenOrder) })
    .AddCustomValidators(new[] { typeof(KitchenOrder) });

builder.Services
    .AddPostgresDbContext<MainDbContext>(
        builder.Configuration.GetConnectionString("kitchendb"),
        null,
        svc => svc.AddRepository(typeof(Repository<>)))
    .AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<KitchenOrderedConsumer>(typeof(KitchenOrderedConsumerDefinition));

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");
        cfg.ConfigureEndpoints(context);
    });
});

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

await app.DoDbMigrationAsync(app.Logger);

app.Run();

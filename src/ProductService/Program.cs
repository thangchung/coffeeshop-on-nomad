using N8T.Infrastructure.OTel;
using Spectre.Console;
using System.Net;
using N8T.Infrastructure;
using ProductService.Domain;
using ProductService.Features;

AnsiConsole.Write(new FigletText("Product APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs();

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    // webBuilder.Limits.MinRequestBodyDataRate = null;
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue<int>("RestPort", 5001)); // REST
    // webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue<int>("GrpcPort", 15001), 
    //     listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; }); // gRPC
});

builder.Services
    .AddHttpContextAccessor()
    .AddCustomMediatR(new[] { typeof(Item) })
    .AddCustomValidators(new[] { typeof(Item) });

// builder.Services.AddGrpc().AddJsonTranscoding();

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

var app = builder.Build();

// app.MapGrpcService<ItemService>();

_ = app.MapItemTypesQueryApiRoutes()
    .MapItemsByIdsQueryApiRoutes();

app.Run();

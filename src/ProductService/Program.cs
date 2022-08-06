using Microsoft.AspNetCore.Server.Kestrel.Core;
using N8T.Infrastructure.OTel;
using ProductService.Infrastructure.Grpc;
using Spectre.Console;
using System.Net;

AnsiConsole.Write(new FigletText("Product APIs").Color(Color.MediumPurple));

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs();

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Limits.MinRequestBodyDataRate = null;
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue<int>("RestPort")); // REST
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue<int>("GrpcPort"), 
        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; }); // gRPC
});

builder.Services.AddGrpc();//.AddJsonTranscoding();

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<ItemService>();

app.Run();

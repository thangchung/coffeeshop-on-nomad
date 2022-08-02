using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProductService.Infrastructure.Grpc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Limits.MinRequestBodyDataRate = null;
    webBuilder.Listen(IPAddress.Any, 5001); // rest
    webBuilder.Listen(IPAddress.Any, 15001, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; }); // grpc
});

builder.Services.AddGrpc().AddJsonTranscoding();

var app = builder.Build();

app.MapGrpcService<ItemService>();
app.MapGet("/", () => "Hello World!");

app.Run();

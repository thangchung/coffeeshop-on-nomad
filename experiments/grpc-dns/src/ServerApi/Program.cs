using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ServerApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Limits.MinRequestBodyDataRate = null;
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue<int>("RestPort")); // REST
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue<int>("GrpcPort"), 
        listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; }); // gRPC
});

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<GreeterService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
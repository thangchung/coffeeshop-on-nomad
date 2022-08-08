using N8T.Infrastructure.OTel;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs();

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 8080)); // REST
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();
app.Run();

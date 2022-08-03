using N8T.Infrastructure.OTel;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.AddOTelLogs();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();
app.Run();

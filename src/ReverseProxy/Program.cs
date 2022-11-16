using System.IdentityModel.Tokens.Jwt;
using N8T.Infrastructure.OTel;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = false,
        SignatureValidator = delegate (string token, TokenValidationParameters _)
        {
            var jwt = new JwtSecurityToken(token);
            return jwt;
        }
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("mypolicy", builder => builder
        .RequireClaim("myCustomClaim", "green")
        .RequireAuthenticatedUser());
    options.FallbackPolicy = null; 
});

builder.WebHost.AddOTelLogs();

builder.WebHost.ConfigureKestrel(webBuilder =>
{
    webBuilder.Listen(IPAddress.Any, builder.Configuration.GetValue("RestPort", 8080)); // REST
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(context =>
    {
        if (string.Equals("myPolicy", context.Route.AuthorizationPolicy))
        {
            context.AddRequestTransform(async transformContext =>
            {
                // AuthN and AuthZ will have already been completed after request routing.
                var ticket = await transformContext.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

                // Reject invalid requests
                if (ticket.Principal?.Claims.Count()! <= 0)
                {
                    var response = transformContext.HttpContext.Response;
                    response.StatusCode = 401;
                }

                // transformContext.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            });
        }
    });

builder.Services.AddOTelTracing(builder.Configuration);
builder.Services.AddOTelMetrics(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();
app.Run();

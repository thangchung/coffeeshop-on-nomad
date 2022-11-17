﻿using System.Security.Claims;
using System.Threading.RateLimiting;

namespace ReverseProxy;

public static class RateLimitExtensions
{
    private static readonly string Policy = "PerUserRatelimit";

    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(Policy, context =>
            {
                // We always have a user id
                var id = context.User.FindFirstValue("id")!;

                return RateLimitPartition.GetTokenBucketLimiter(id, key =>
                    new TokenBucketRateLimiterOptions
                    {
                        ReplenishmentPeriod = TimeSpan.FromSeconds(5),
                        AutoReplenishment = true,
                        TokenLimit = 100,
                        TokensPerPeriod = 100,
                        QueueLimit = 100,
                    });
            });
        });
    }

    public static IEndpointConventionBuilder RequirePerUserRateLimit(this IEndpointConventionBuilder builder)
    {
        return builder.RequireRateLimiting(Policy);
    }
}
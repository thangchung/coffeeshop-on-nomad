using System;
using System.ComponentModel;
using System.Diagnostics;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using N8T.Infrastructure.Logging;
using N8T.Infrastructure.Validator;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace N8T.Infrastructure
{

    public static class Extensions
    {
        [DebuggerStepThrough]
        public static IServiceCollection AddCustomMediatR(this IServiceCollection services, Type[] types = null,
            Action<IServiceCollection> doMoreActions = null)
        {
            services.AddHttpContextAccessor();

            services.AddMediatR(types)
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            doMoreActions?.Invoke(services);

            return services;
        }

        [DebuggerStepThrough]
        public static IServiceCollection AddCustomValidators(this IServiceCollection services, Type[] types)
        {
            return services.Scan(scan => scan
                .FromAssembliesOf(types)
                .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            //return services.AddValidatorsFromAssemblies(types.Select(t => t.Assembly));
        }

        public static IServiceCollection AddCustomDaprClient(this IServiceCollection services)
        {
            services.AddDaprClient();
            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services, string corsName = "api",
            Action<CorsOptions> optionsAction = null)
        {
            services.AddCors(options =>
            {
                if (optionsAction == null)
                {
                    options.AddPolicy(corsName,
                        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
                }
                else
                {
                    optionsAction.Invoke(options);
                }
            });

            return services;
        }

        public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app, string corsName = "api")
        {
            return app.UseCors(corsName);
        }

        [DebuggerStepThrough]
        public static T ConvertTo<T>(this string input)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                return (T)converter.ConvertFromString(input);
            }
            catch (NotSupportedException)
            {
                return default;
            }
        }
    }
}

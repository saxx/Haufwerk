using System;
using System.Globalization;
using JetBrains.Annotations;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.AspNet.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haufwerk.Client
{
    public static class HaufwerkExtensions
    {
        public static void AddHaufwerk([NotNull] this IServiceCollection services, [NotNull] HaufwerkOptions options)
        {
            services.AddSingleton(provider => options);
            services.AddTransient<IHaufwerk, Haufwerk>();
            services.AddTransient<HaufwerkErrorLoggerProvider>();
        }


        public static void AddHaufwerk([NotNull] this IServiceCollection services, [NotNull] string source, [NotNull] string haufwerkInstanceUri)
        {
            AddHaufwerk(services, new HaufwerkOptions(source, haufwerkInstanceUri));
        }


        public static IApplicationBuilder UseHaufwerk([NotNull] this IApplicationBuilder app, [NotNull] string locationFormat)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(app.ApplicationServices.GetService<HaufwerkErrorLoggerProvider>());

            app.UseExceptionHandler(exceptionBuilder =>
            {
                exceptionBuilder.Run(async context =>
                {
                    try
                    {
                        var haufwerk = app.ApplicationServices.GetRequiredService<IHaufwerk>();
                        var error = context.Features.Get<IExceptionHandlerFeature>();

                        if (!haufwerk.Options.LogLocalRequests && Haufwerk.IsLocalRequest(context.Request))
                        {
                            // just return the full error message
                            await context.Response.WriteAsync(error.Error.ToString());
                        }
                        else
                        {
                            // log to Haufwerk and redirect to the error page
                            var requestUrl = context.Request?.GetDisplayUrl();
                            await haufwerk.Post(haufwerk.Options.Source, error.Error.Message, null, error.Error.ToString(), $"Request URL: {requestUrl}");

                            var location = string.Format(CultureInfo.InvariantCulture, locationFormat, context.Response.StatusCode);
                            if (locationFormat.StartsWith("~") && context.Request != null)
                            {
                                location = context.Request.PathBase + string.Format(CultureInfo.InvariantCulture, locationFormat.Substring(1), context.Response.StatusCode);
                            }
                            context.Response.Redirect(location);
                        }
                    }
                    catch (Exception ex)
                    {
                        await context.Response.WriteAsync(ex.ToString());
                    }
                });
            });

            return app;
        }
    }
}
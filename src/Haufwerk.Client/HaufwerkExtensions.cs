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
        /// <summary>
        /// Add Haufwerk-related classes to the service container.
        /// </summary>
        /// <param name="services">The service container.</param>
        /// <param name="options">The configuration options for Haufwerk.</param>
        public static void AddHaufwerk([NotNull] this IServiceCollection services, [NotNull] HaufwerkOptions options)
        {
            services.AddSingleton(provider => options);
            services.AddTransient<IHaufwerk, Haufwerk>();
            services.AddTransient<HaufwerkErrorLoggerProvider>();
        }


        /// <summary>
        /// Add Haufwerk-related classes to the service container.
        /// </summary>
        /// <param name="services">The service container</param>
        /// <param name="source">A name that is used to identify the error source (ie. the name of your project).</param>
        /// <param name="haufwerkInstanceUri">The URL of your Haufwerk instance (ie. https://haufwerk.server.com)</param>
        public static void AddHaufwerk([NotNull] this IServiceCollection services, [NotNull] string source, [NotNull] string haufwerkInstanceUri)
        {
            AddHaufwerk(services, new HaufwerkOptions(source, haufwerkInstanceUri));
        }


        /// <summary>
        /// Catches all application exceptions and posts them to Haufwerk. 
        /// In addition, when using the default configuration, the stacktrace is printed for local requests and a generic error message is printed for remote requests.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <returns>The updated application builder (for fluent method chaining).</returns>
        public static IApplicationBuilder UseHaufwerk([NotNull] this IApplicationBuilder app)
        {
            return UseHaufwerk(app, null);
        }

        /// <summary>
        /// Catches all application exceptions and posts them to Haufwerk. 
        /// In addition, when using the default configuration, the stacktrace is printed for local requests and a generic error message is printed for remote requests.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="locationFormat">
        /// If specified, ASP.NET is configured for 'UseStatusCodePagesWithRedirects' using the specified locationFormat. 
        /// In that case, the generic error message behavior is overridden.
        /// </param>
        /// <returns>The updated application builder (for fluent method chaining).</returns>
        public static IApplicationBuilder UseHaufwerk([NotNull] this IApplicationBuilder app, [CanBeNull] string locationFormat)
        {
            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(app.ApplicationServices.GetService<HaufwerkErrorLoggerProvider>());

            if (!string.IsNullOrWhiteSpace(locationFormat))
            {
                app.UseStatusCodePagesWithRedirects(locationFormat);
            }

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
                            context.Response.StatusCode = 500;
                            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = error.Error.Message;
                            await context.Response.WriteAsync(Haufwerk.ToAsyncStringSafe(error.Error));
                        }
                        else
                        {
                            // log to Haufwerk and redirect to the error page
                            var requestUrl = context.Request?.GetDisplayUrl();
                            await haufwerk.Post(haufwerk.Options.Source, error.Error, null, $"Request URL: {requestUrl}");

                            if (!string.IsNullOrWhiteSpace(locationFormat))
                            {
                                var location = string.Format(CultureInfo.InvariantCulture, locationFormat, context.Response.StatusCode);
                                if (locationFormat.StartsWith("~") && context.Request != null)
                                {
                                    location = context.Request.PathBase + string.Format(CultureInfo.InvariantCulture, locationFormat.Substring(1), context.Response.StatusCode);
                                }
                                context.Response.Redirect(location);
                            }
                            else
                            {
                                // hide the full error message
                                context.Response.StatusCode = 500;
                                await context.Response.WriteAsync("An error has occurred (HTTP 500).");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        await context.Response.WriteAsync(Haufwerk.ToAsyncStringSafe(ex));
                    }
                });
            });

            return app;
        }
    }
}
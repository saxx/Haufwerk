using Haufwerk.Client;
using Haufwerk.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haufwerk
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            _isDevEnvironment = env.IsDevelopment();

            var builder = new ConfigurationBuilder()
                .AddJsonFile("Haufwerk.json")
                .AddEnvironmentVariables("Haufwerk:");
            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(true);
            }
            Configuration = builder.Build();
        }

        private readonly bool _isDevEnvironment = false;
        public IConfigurationRoot Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationInsightsTelemetry(Configuration);

            if (_isDevEnvironment)
            {
                services.AddHaufwerk(new HaufwerkOptions("Haufwerk", "http://localhost:5000")
                {
                    LogLocalRequests = true
                });
            }
            else
            {
                services.AddHaufwerk(new HaufwerkOptions("Haufwerk", "https://haufwerk.sachsenhofer.com")
                {
                    LogLocalRequests = true
                });
            }

            services.AddMvc();
            services.AddDbContext<Db>(options =>
            {
                options.UseSqlServer(Configuration["Database:ConnectionString"]);
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseHaufwerk();

            app.ApplicationServices
                .GetService<Db>()
                .Database
                .Migrate();

            app.UseStaticFiles(new StaticFileOptions());

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

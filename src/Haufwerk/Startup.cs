using Haufwerk.Client;
using Haufwerk.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Haufwerk
{
    public class Startup
    {
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        // ReSharper disable once UnusedParameter.Local
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("Haufwerk.json")
                .AddEnvironmentVariables("Haufwerk:");
            Configuration = builder.Build();
        }


        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHaufwerk(new HaufwerkOptions("Haufwerk", "http://localhost:5000")
            {
                LogLocalRequests = true
            });
            services.AddMvc();
            services.AddEntityFramework().AddSqlite().AddDbContext<Db>(options =>
            {
                options.UseSqlite(Configuration.GetSection("Database").Get("ConnectionString", "Data Source=Haufwerk.db"));
            });
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseHaufwerk("~/error/{0}");

            app.ApplicationServices
                .GetService<Db>()
                .Database
                .EnsureCreated();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

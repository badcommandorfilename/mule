using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Mule
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        public IServiceCollection Services { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Services = services;
            var connection = "Filename=db.sqlite";//Configuration.GetConnectionString("SQLite");

            Services.AddEntityFrameworkSqlite()
                .AddDbContext<RepositoryContext>(
                    options => options.UseSqlite(connection)
                );

            Services.AddMvc();
            Services.AddMemoryCache();

            //Scoped services are injected into constructors that match the interface
            foreach (var t in ModelService.AllModels())
            {
                Type unbound_interface = typeof(IRepository<>); //Interface for type construction
                Type unbound_instance = typeof(Repository<>); //Constructed service class for dependency injection
                Services.AddScoped(
                    unbound_interface.MakeGenericType(t), //Bind reflected model type to interface
                    unbound_instance.MakeGenericType(t) //Bind reflected model type to service
                );
            }
            

            //Singleton services are created on first injection and re-used
            var scheduler = Chroniton.Singularity.Instance; //Background task scheduler
            Services.AddSingleton<Chroniton.ISingularity>(scheduler);
            scheduler.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStatusCodePagesWithRedirects("~/error/{0}"); //Client error page (e.g. for 404)
            app.UseDeveloperExceptionPage(); //Internal exception page for debugging

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                    context.Context.Response.Headers.Add("Cache-Control", "public, max-age=86400")
            });

            app.UseMvc();
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = "Filename=db.sqlite";//Configuration.GetConnectionString("SQLite");

            services.AddEntityFramework()
                .AddDbContext<SQLiteContext>(options =>
                    options.UseSqlite(connection)
                );

            services.AddMvc();
            services.AddMemoryCache();

            //Scoped services are injected into constructors that match the interface
            services.AddScoped<IRepository<AppHost>, Repository<AppHost>>();

            //Singleton services are created on first injection and re-used
            var scheduler = Chroniton.Singularity.Instance; //Background task scheduler
            services.AddSingleton<Chroniton.ISingularity>(scheduler);
            scheduler.Start();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                context.Context.Response.Headers.Add("Cache-Control", "public, max-age=86400")
            });

            app.UseMvc();
        }
    }
}

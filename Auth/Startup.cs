using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Auth
{
    public class Startup
    {
        public IConfiguration Configuration {get;}

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("db");
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            
            services.AddIdentityServer()
                    .AddConfigurationStore(options => {
                        options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                    })
                    .AddOperationalStore(options => {
                        options.ConfigureDbContext = builder => builder.UseNpgsql(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
                    })
                    .AddTestUsers(Config.Users)
                    .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}

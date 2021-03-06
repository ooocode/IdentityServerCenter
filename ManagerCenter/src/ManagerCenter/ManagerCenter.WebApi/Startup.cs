using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ManagerCenter.UserManager.EntityFrameworkCore.Extensions;
using ManagerCenter.Shared.Database;

namespace ManagerCenter.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddControllers();
            services.AddUserManager(DatabaseType.Sqlite, "data source=TempDb/aa.db", "data source=TempDb/config.db", "data source=TempDb/opt.db");
            services.AddOpenApiDocument(); // add OpenAPI v3 document

            services.AddDataProtection();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(e => e.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI
            app.UseReDoc(config => config.Path = "/redoc"); // serve ReDoc UI
        }
    }
}

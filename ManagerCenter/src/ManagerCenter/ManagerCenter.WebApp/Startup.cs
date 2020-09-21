using Elsa.Activities.Http.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.WebApp
{
    public class ElsaContextEx: ElsaContext
       
    {
        public ElsaContextEx(DbContextOptions<ElsaContextEx> options)
            :base(options)
        {

        }
    }
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
            services.AddRazorPages().AddRazorRuntimeCompilation();
          
            services.AddDbContextPool<ElsaContextEx>(ctx => { ctx.UseSqlServer("server=127.0.0.1;database=alse;user=sa;password=12345678;"); });
            services.BuildServiceProvider().GetService<ElsaContextEx>().Database.EnsureCreated();
          services
               // Add Elsa services. 
               .AddElsa(
                   elsa =>
                   {
                       elsa.AddEntityFrameworkStores<ElsaContextEx>(ctx => { });
                       // Configure Elsa to use the MongoDB provider.
                       //elsa.AddEntityFrameworkStores(Configuration, databaseName: "UserRegistration", connectionStringName: "MongoDb");
                   })

        // Add Elsa Dashboard services.
        // Registers necessary service to handle HTTP requests.
        .AddHttpActivities()

               .AddElsaDashboard();

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //var s = app.ApplicationServices.CreateScope();
           // s.ServiceProvider.GetService<ElsaContextEx>().Database.EnsureDeleted();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            // Add Elsa's middleware to handle HTTP requests to workflows.  
            app.UseHttpActivities();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}

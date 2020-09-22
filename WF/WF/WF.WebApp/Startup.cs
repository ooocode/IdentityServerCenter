using Camunda.Api.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using WF.Core;
using WF.Core.Data;
using WF.WebApp.Models;
using WF.WebApp.Services;

namespace WF.WebApp
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
            services.AddArchManager<MyArch>();

            services.AddControllers();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddSingleton(ctx =>
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("http://localhost:8080/engine-rest");
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic ZGVtbzpkZW1v");
                CamundaClient camunda = CamundaClient.Create(httpClient);
                return camunda;
            });

            services.AddHttpClient<TasksService>(ctx => ctx.BaseAddress = new Uri("http://127.0.0.1:8080"));
            services.AddCors();
            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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

            app.UseCors(e => e.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseReDoc();
        }
    }
}

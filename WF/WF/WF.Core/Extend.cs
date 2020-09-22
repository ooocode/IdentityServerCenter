using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using WF.Core.Data;
using WF.Core.Managers;
using WF.Core.Models;

namespace WF.Core
{
    public static class Extend
    {
        public static void AddArchManager<TArch>(this IServiceCollection services) where TArch:Arch
        {
            services.AddDbContextPool<ArchDbContext<TArch>>(ctx => ctx.UseSqlite("data source=arch.db"));
            services.BuildServiceProvider().GetService<ArchDbContext<TArch>>().Database.EnsureCreated();
            services.AddScoped<ArchManager<TArch>>();
        }
    }
}

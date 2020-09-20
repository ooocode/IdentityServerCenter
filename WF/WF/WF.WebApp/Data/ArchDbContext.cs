using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.WebApp.Models;

namespace WF.WebApp.Data
{
    public class ArchDbContext : DbContext
    {
        public ArchDbContext(DbContextOptions<ArchDbContext> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public DbSet<Arch> Arches { get; set; }
    }
}

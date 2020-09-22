using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.Core.Models;

namespace WF.Core.Data
{
    public class ArchDbContext<TArch> : DbContext where TArch : Arch
    {
        public ArchDbContext(DbContextOptions<ArchDbContext<TArch>> dbContextOptions)
            : base(dbContextOptions)
        {

        }

        public DbSet<TArch> Arches { get; set; }

        public DbSet<ArchAttachment> ArchAttachments  { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TArch>().HasKey(e => e.Id);
            modelBuilder.Entity<TArch>().HasIndex(e => e.BusinessKey).IsUnique();

            //附件
            modelBuilder.Entity<ArchAttachment>().HasIndex(e => e.BusinessKey);
            modelBuilder.Entity<ArchAttachment>().HasIndex(e => e.FileName);
        }
    }
}

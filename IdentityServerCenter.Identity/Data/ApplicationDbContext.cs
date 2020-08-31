using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityServerCenter.Models;
using IdentityServerCenter.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServerCenter.Data
{
    //TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole,string,
         ApplicationIdentityUserClaim, IdentityUserRole<string>, IdentityUserLogin<string>, ApplicationIdentityRoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 学校班级
        /// </summary>
        public DbSet<SchoolClass> SchoolClasses { get; set; }

        /// <summary>
        /// 权限
        /// </summary>
        public DbSet<Permisson> Permissons { get; set; }

        /// <summary>
        /// 角色权限
        /// </summary>
        public DbSet<RolePermisson> RolePermissons  { get; set; }

        /// <summary>
        /// 字典类型
        /// </summary>
        public DbSet<DictionaryType> DictionaryTypes  { get; set; }

        /// <summary>
        /// 字典
        /// </summary>
        public DbSet<Dictionary> Dictionaries  { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            base.OnModelCreating(builder);
          

            //设置学校班级表 班级名称唯一
            builder.Entity<SchoolClass>().HasIndex(e => new { e.Name }).IsUnique();

            //声明类型的ClientId和名称构成唯一索引
            builder.Entity<Permisson>().HasIndex(e => e.Name).IsUnique();

           
            builder.Entity<RolePermisson>().HasKey(e => new { e.RoleId, e.PermissonId });

            //字典类型名称唯一
            builder.Entity<DictionaryType>().HasIndex(e => e.Name).IsUnique();

            //目录类型 和Code 是唯一索引
            builder.Entity<Dictionary>().HasIndex(e => new { e.DictionaryTypeId, e.Code }).IsUnique();
        }
    }
}

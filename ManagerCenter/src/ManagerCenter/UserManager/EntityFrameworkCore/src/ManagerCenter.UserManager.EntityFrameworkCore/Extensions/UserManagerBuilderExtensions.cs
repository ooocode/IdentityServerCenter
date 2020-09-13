using AutoMapper;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerCenter.UserManager.EntityFrameworkCore.Extensions
{
    public static class UserManagerBuilderExtensions
    {
        public static void AddUserManager(this IServiceCollection services, DatabaseType databaseType,string connectString)
        {

            services.AddDbContextPool<ApplicationDbContext>(options => options.UseDatabase(databaseType, connectString));
            services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.EnsureCreated();

            services.AddIdentityCore<ApplicationUser>(
                config =>
                {
                    config.Password = new PasswordOptions
                    {
                        RequireDigit = false,
                        RequiredLength = 6,
                        RequiredUniqueChars = 0,
                        RequireLowercase = false,
                        RequireNonAlphanumeric = false,
                        RequireUppercase = false
                    };

                    //如果连续10次输入密码都不对，则在第10次以后锁定账号，锁定默认是5分钟
                    config.Lockout.MaxFailedAccessAttempts = 10;

                    //config.SignIn.RequireConfirmedEmail = true;
                    //config.User.RequireUniqueEmail = true;
                })
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
            //.AddErrorDescriber<AppIdentityErrorDescriber>()
            //.AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(UserManagerBuilderExtensions).Assembly);


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IDictionaryService, DictionaryService>();
            services.AddScoped<IPermissonService, PermissonService>();
            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}

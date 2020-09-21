using AutoMapper;
using ManagerCenter.Shared.Database;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.Abstractions.Models.UserManagerModels;
using ManagerCenter.UserManager.Abstractions.UserManagerInterfaces;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ManagerCenter.UserManager.EntityFrameworkCore.Extensions
{
    public static class UserManagerBuilderExtensions
    {
        public static void AddUserManager(this IServiceCollection services,
            DatabaseType databaseType,
            string connectString,
             string configurationStoreDbConnectString,
             string operationalStoreDbConnectString)
        {

            services.AddDbContextPool<ApplicationDbContext>(options => options.UseDatabase(databaseType, connectString));
            services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.EnsureCreated();

            services.AddIdentity<ApplicationUser, ApplicationRole>(
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
                .AddUserManager<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>()
            //.AddErrorDescriber<AppIdentityErrorDescriber>()
            .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(UserManagerBuilderExtensions).Assembly);


            services.AddIdentityServer()
               .AddAspNetIdentity<ApplicationUser>()
               .AddConfigurationStore(Config =>
               {
                   Config.ConfigureDbContext = build =>
                  build.UseDatabase(databaseType, configurationStoreDbConnectString);
               })
               .AddOperationalStore(config =>
               {
                   config.ConfigureDbContext = builder => builder.UseDatabase(databaseType, operationalStoreDbConnectString);
                   config.EnableTokenCleanup = true;
               });


            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            //services.AddScoped<IDictionaryService, DictionaryService>();
            services.AddScoped<IPermissonService, PermissonService>();
            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}

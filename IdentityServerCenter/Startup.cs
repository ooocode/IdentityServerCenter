using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Database.Services.DefaultImpl;
using IdentityServerCenter.Models;
using IdentityServerCenter.Services.ApiResourceService;
using IdentityServerCenter.Services.ClientService;
using IdentityServerCenter.Services.IdentityResourceService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Study.Service;
using System.IO;
using FluentValidation.AspNetCore;
using IdentityServerCenter.ViewModels;
using IdentityServerCenter.Extensions;
using Microsoft.AspNetCore.DataProtection;

namespace IdentityServerCenter
{
 
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;

            IdentityModelEventSource.ShowPII = true;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
                //iis.AuthenticationDisplayName = "Windows";
            });

            // configures IIS in-proc settings
            services.Configure<IISServerOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            DatabaseType databaseType = DatabaseType.Sqlite;
            if (Environment.IsProduction())
            {
                databaseType = DatabaseType.MSSqlServer;
            }
            services.AddDbContextPool<ApplicationDbContext>(options => options.UseDatabase(databaseType, Configuration.GetConnectionString("DefaultConnection")));

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
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManagerEx>()
                .AddErrorDescriber<AppIdentityErrorDescriber>()
                .AddDefaultTokenProviders();

       

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                //options.Endpoints.EnableCheckSessionEndpoint = false;

            }).AddAspNetIdentity<ApplicationUser>()
                .AddConfigurationStore(Config =>
                    Config.ConfigureDbContext = build =>
                    build.UseDatabase(databaseType, Configuration.GetConnectionString("ConfigureDbContext")))
                .AddOperationalStore(config =>
                   {
                       config.ConfigureDbContext = builder => builder.UseDatabase(databaseType, Configuration.GetConnectionString("PersistedGrantDbContext"));
                       config.EnableTokenCleanup = true;
                   })
                .AddProfileService<ProfileServiceEx>();

            //builder.AddApiAuthorization<ApplicationUser,>();
            //微软包装的API授权 注入 IClientRequestParametersProvider  可以根据客户端ID返回客户端信息
            //services.AddScoped<PersistedGrantDbContext>();

            //builder.AddApiAuthorization<ApplicationUser, PersistedGrantDbContext>();
            //builder.AddApiAuthorization<ApplicationUser, PersistedGrantDbContext>();

            builder.AddDeveloperSigningCredential(/*filename:"qq.rsa"*/);
            //if (Environment.IsDevelopment())
            //{
            //    builder.AddDeveloperSigningCredential();
            //}
            //else
            {
                //builder.AddDeveloperSigningCredential(filename: "tempkey.rsa");
                //using var cert = new X509Certificate2("SHA256withRSA_zwovo.xyz.pfx","651398");
                //builder.AddSigningCredential(new X509Certificate2("SHA256withRSA_zwovo.xyz.pfx", "651398"));
            }


            services.BuildServiceProvider().GetService<ConfigurationDbContext>().Database.EnsureCreated();
            services.BuildServiceProvider().GetService<PersistedGrantDbContext>().Database.EnsureCreated();
            services.BuildServiceProvider().GetService<ApplicationDbContext>().Database.EnsureCreated();



            var authen = services.AddAuthentication()
                   //.AddGoogle(options =>
                   //{
                   //  // register your IdentityServer with Google at https://console.developers.google.com
                   //  // enable the Google+ API
                   //  // set the redirect URI to http://localhost:5000/signin-google
                   //  options.ClientId = "copy client ID from Google here";
                   //    options.ClientSecret = "copy client secret from Google here";
                   //})
                   ;
            var GithubClientEnable = bool.Parse(Configuration["GithubClientEnable"]);
            var GithubClientId = Configuration["GithubClientId"];
            var GithubClientPassword = Configuration["GithubClientPassword"];
            if (GithubClientEnable)
            {
                authen.AddGitHub(options =>
                {
                    options.ClientId = GithubClientId;
                    options.ClientSecret = GithubClientPassword;
                });
            }

            authen.AddQQ(qqOpt =>
            {
                qqOpt.ClientId = "101809620";
                qqOpt.ClientSecret = "26d4f34704c6ea99ecd58a15b27c99b9";
            });


            services.AddAuthorization(conf =>
            {
                conf.AddPolicy(SeedData.defaultSuperAdminRole, policy => policy.RequireRole(SeedData.defaultSuperAdminRole).Build());
            });


            services.AddControllersWithViews();
            services.AddRazorPages().AddRazorRuntimeCompilation().AddRazorPagesOptions(opt =>
            {
                opt.Conventions.AuthorizeFolder("/UserPage", SeedData.defaultSuperAdminRole);
                opt.Conventions.AddPageRoute("/UserPage/UserManagerPages/Index", "");

                //opt.Conventions.AddPageRoute("/swagger/index.html", "");
            });


            services.AddMvc().AddNewtonsoftJson().AddFluentValidation(opt=> {
                opt.RegisterValidatorsFromAssemblyContaining<ApplicationUserValidator>();
                opt.RegisterValidatorsFromAssemblyContaining<CreateOrUpdateUserViewModel>();
                opt.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
            });
            //    .AddJsonOptions(opt=> 
            //{
            //    //opt.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
            //    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
            //});

            //if (Environment.IsDevelopment())
            //{
            //    services.AddDistributedMemoryCache();
            //}
            //else
            //{
            //    services.AddDistributedRedisCache(e => e.Configuration = "127.0.0.1:6379");
            //}

            services.AddDistributedMemoryCache();
            services.AddSingleton<CacheService>();

            services.AddGrpc();
          
            //services.AddGrpcWeb(ctx => ctx.GrpcWebEnabled = true);

            services.AddMiniProfiler()
            .AddEntityFramework();

            //services.AddDataProtection(opt =>
            //{
            //    opt.ApplicationDiscriminator = "Study";
            //})
            //   .PersistKeysToFileSystem(new DirectoryInfo(@"C:\temp-keys\"))
            //   .ProtectKeysWithCertificate(new X509Certificate2("SHA256withRSA_zwovo.xyz.pfx", "651398"));

            if (!Directory.Exists(Configuration["AvatarSavePath"]))
            {
                Directory.CreateDirectory(Configuration["AvatarSavePath"]);
            }

            

            services.AddCors();


            services.AddAutoMapper(typeof(Startup).Assembly, typeof(ApplicationDbContext).Assembly);


            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IDictionaryService, DictionaryService>();
            services.AddScoped<IPermissonService, PermissonService>();
            services.AddScoped<IIdentityResourceService, IdentityResourceService>();
            services.AddScoped<IApiResourceService, ApiResourceService>();
            services.AddScoped<IApiScopeService, ApiScopeService>();
  
            
            services.AddOpenApiDocument(); // add OpenAPI v3 document
            services.AddApplicationInsightsTelemetry();
            //      services.AddSwaggerDocument(); // add Swagger v2 document

            services.AddScoped<IEmailSender, EmailSender>();

            //services.AddCap(x =>
            //{
            //    // Register Dashboard
            //    x.UseDashboard(opt=>
            //    {
                   
            //    });

            //    x.UseEntityFramework<ApplicationDbContext>();

            //    x.UseKafka(opt =>
            //    {
            //        opt.Servers = "127.0.0.1:9092";
            //    });
            //    //x.UseRabbitMQ(opt =>
            //    //{
            //    //    opt.HostName = "localhost";
            //    //    opt.UserName = "guest";
            //    //    opt.Password = "guest";
            //    //});
            //});


            services.AddAuthentication()
                .AddIdentityServerAuthentication("usergateway", options =>
                {
                    options.Authority = "https://localhost:9999";
                   
                    //options.ApiName = "api1";
                    //options.ApiSecret = "liyouming";
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(conf =>
            {
                conf.AddPolicy("requeredAdmin", policy =>
                {
                    policy.RequireAuthenticatedUser().RequireRole(SeedData.defaultSuperAdminRole)
                    .AddAuthenticationSchemes("usergateway")
                    .Build();
                });
            });

            services.AddDataProtection()
                .SetApplicationName("IdentityServer")
                .PersistKeysToFileSystem(new DirectoryInfo(Configuration["DataProtectionPersistKeysDirectory"]))
                .ProtectKeysWithCertificate(Configuration["X509Thumbprint"]);
        }

        public void Configure(IApplicationBuilder app, ConfigurationDbContext configurationDbContext,
            ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            //初始化用户
            SeedData seedData = new SeedData(configurationDbContext, applicationDbContext, userManager);
            seedData.InitAsync().Wait();

            if (Environment.IsDevelopment())
            {
                // ...existing configuration...
                app.UseMiniProfiler();

                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(e => e.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            //app.UseCapDashboard();

            app.UseCookiePolicy();

            //全局日志记录
            //app.Use((context, next) =>
            //{
            //    var ip = context.Connection.RemoteIpAddress.ToString();
            //    var url = context.Request.Path;
            //    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();

            //    logger.LogInformation($"IP:{ip} 请求地址:{url} 查询字符串:{context.Request.QueryString}");

            //    return next.Invoke();
            //});

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseGrpcWeb();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<Rpc.ProfileServiceEx>().EnableGrpcWeb();

                endpoints.MapControllers()/*.RequireAuthorization("requeredAdmin")*/;
                endpoints.MapRazorPages();
            });

            app.UseOpenApi(); // serve OpenAPI/Swagger documents
            app.UseSwaggerUi3(); // serve Swagger UI
            app.UseReDoc(config =>config.Path="/redoc"); // serve ReDoc UI
        }
    }
}
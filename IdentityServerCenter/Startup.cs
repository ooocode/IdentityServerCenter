using AutoMapper;
using DotNetCore.CAP;
using Extensions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Events;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Database.Services.DefaultImpl;
using IdentityServerCenter.Models;
using IdentityServerCenter.Services.ApiResourceService;
using IdentityServerCenter.Services.ClientService;
using IdentityServerCenter.Services.IdentityResourceService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Org.BouncyCastle.Crypto.Signers;
using Study.Service;
using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation.AspNetCore;
using IdentityServerCenter.Database.Dtos.UserServiceDtos;
using IdentityServerCenter.ViewModels;

namespace IdentityServerCenter
{

    public class LongJsonConverter : System.Text.Json.Serialization.JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                ReadOnlySpan<byte> span = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan;
                if (Utf8Parser.TryParse(span, out long number, out int bytesConsumed) && span.Length == bytesConsumed)
                    return number;

                if (Int64.TryParse(reader.GetString(), out number))
                    return number;
            }

          

            return reader.GetInt64();
        }

        //public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        //{
        //    return reader.GetInt64();
        //}

        public override void Write(Utf8JsonWriter writer, [DisallowNull] long value, JsonSerializerOptions options)
        {
            if (writer is null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            writer.WriteStringValue($"{value}");
        }
    }

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

        private void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                //var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.
                //if (/* UserAgent doesn’t support new behavior */)
                {
                    // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // cookie policy to deal with temporary browser incompatibilities
            //services.AddSameSiteCookiePolicy();

            //services.Configure<KestrelServerOptions>(ctx => 
            //{
            //    var con = Configuration["PfxFingerprint"];
            //    var cc = System.Text.Encoding.UTF8.GetBytes(con);
            //    ctx.ListenAnyIP(9998, op => { op.Protocols = HttpProtocols.Http2; });

            //    ctx.ListenAnyIP(9999, op =>
            //    {
            //        op.UseHttps(e => e.ServerCertificate = new X509Certificate2());
            //    });

            //    ctx.ListenAnyIP(10000, op =>
            //    {
            //        op.Protocols = HttpProtocols.Http2;

            //        op.UseHttps(e => e.ServerCertificate = new X509Certificate2());
            //    });
            //});


            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });


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

            services.AddDbContextPool<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), opt => { 
                    opt.EnableRetryOnFailure();
                }));


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
                    build.UseSqlServer(Configuration.GetConnectionString("ConfigureDbContext"),
                    opt =>
                    {
                        opt.EnableRetryOnFailure();

                        //opt.EnableSensitiveDataLogging();
                        //opt.MigrationsAssembly("IdentityServerCenter");
                    }))
                .AddOperationalStore(config =>
                   {
                       config.ConfigureDbContext = builder => builder.UseSqlServer(Configuration.GetConnectionString("PersistedGrantDbContext"),
                           opt =>
                           {
                               opt.EnableRetryOnFailure();
                           });
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

            services.AddScoped<IClientService, ClientService>();

            services.AddCors();


            services.AddAutoMapper(typeof(Startup).Assembly, typeof(ApplicationDbContext).Assembly);

            //services.AddAutoMapper(typeof(Startup).Assembly, typeof(ApplicationDbContext).Assembly);
            //services.AddDbContext<QQDbContext>();

            // services.BuildServiceProvider().GetService<QQDbContext>().Database.EnsureCreated();
            //services.BuildServiceProvider().GetService<QQDbContext>().Database.EnsureCreated();

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
        }

        public void Configure(IApplicationBuilder app, ConfigurationDbContext configurationDbContext,
            ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager)
        {
            //初始化用户
            SeedData seedData = new IdentityServerCenter.Models.SeedData(configurationDbContext, applicationDbContext, userManager);
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
            app.UseReDoc(); // serve ReDoc UI
        }
    }
}
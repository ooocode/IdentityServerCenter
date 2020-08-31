using Grpc.Core;
using IdentityModel;
using IdentityServerCenterConnect;
using IdentityServerCenterConnect.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Study.Website.Controllers;
//using Study.WebsiteIdentityServerCenterConnect.Authorization;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServerCenterConnectServiceCollectionExtensions
    {
  
        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite == SameSiteMode.None)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                // TODO: Use your User Agent library of choice here.
                //if (/* UserAgent doesn’t support new behavior */)
                {
                    // For .NET Core < 3.1 set SameSite = (SameSiteMode)(-1)
                    options.SameSite = SameSiteMode.Unspecified;
                }
            }
        }

        public static ConfigurationOptions ConfigurationOptions = null;

        private static string DefaultScheme = "AspNetCore.Identity.Application";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="userPermissionCacheTimeSpan">用户权限缓存时间缓存</param>
        public static void AddIdentityServerCenterConnect(this IServiceCollection services, ConfigurationOptions configuration)
        {
            ConfigurationOptions = configuration;
            //配置cookie
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                options.OnAppendCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext =>
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });


            //清空默认绑定的用户信息
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //添加认证服务
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // CookieAuthenticationDefaults.AuthenticationScheme; //默认使用Cookies方案进行认证
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;        //默认认证失败时启用oidc方案
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;   // CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(/*CookieAuthenticationDefaults.AuthenticationScheme*/)
            //添加oidc方案
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;  //身份验证成功后使用Cookies方案来保存信息

                options.Authority = configuration.AuthorityUrl;    //授权服务地址
                options.RequireHttpsMetadata = false;

                options.ClientId = configuration.ClientId;
                options.ClientSecret = configuration.ClientSecret;

                options.ResponseType = $"{OidcConstants.ResponseTypes.IdToken} {OidcConstants.ResponseTypes.Token}";

                options.SaveTokens = true;


                options.GetClaimsFromUserInfoEndpoint = true;
            //options.RemoteAuthenticationTimeout = TimeSpan.FromSeconds(10);

                options.TokenValidationParameters = new TokenValidationParameters
                {

                    NameClaimType = JwtClaimTypes.Name,
                    RoleClaimType = JwtClaimTypes.Role,
                };

                options.ResponseMode = OidcConstants.ResponseModes.FormPost;

                string[] deleteClaims = { "s_hash", "auth_time" ,"sid",
                    "idp", "amr", "AspNet.Identity.SecurityStamp" };
                foreach (var deleteClaim in deleteClaims)
                {
                    options.ClaimActions.DeleteClaim(deleteClaim);
                }


                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = context =>
                    {
                    //var logger = context.HttpContext.RequestServices.GetService<ILogger<IdentityServerCenterConnectServiceCollectionExtensions>>();
                    //logger.LogError($"OIDC发生错误：{context.Failure.InnerException?.Message ?? context.Failure.Message}");

                    context.Response.Redirect("/");
                        context.HandleResponse();

                        return Task.FromResult(0);
                    },

                    OnMessageReceived = ctx =>
                    {
                        var idToken = ctx.ProtocolMessage.IdToken;
                        if (!string.IsNullOrEmpty(idToken))
                        {
                            Console.WriteLine("****************【OnMessageReceived】*****************");
                            Console.WriteLine("IdToken:" + idToken);

                            var principal = new ClaimsPrincipal();
                            var token = new JwtSecurityToken(idToken);

                            var identity = new ClaimsIdentity();
                            identity.AddClaims(token.Claims);
                            principal.AddIdentity(identity);
                            ctx.HttpContext.SignInAsync(principal);
                        }
                        return Task.CompletedTask;
                    }
                };

                foreach (var item in configuration.Scopes)
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        options.Scope.Add(item);
                    }
                }
            });

            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextHelper, HttpContextHelper>();


            // This switch must be set before creating the GrpcChannel/HttpClient.
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            services.AddGrpcClient<UserGrpcService.User.UserClient>(ctx=>ctx.Address = new Uri(configuration.GRpcUrl));
            services.AddSingleton<IAuthorizationPolicyProvider, AppAuthorizationPolicyProvider>();
        }


        //app.UseRouting()之后加上
        //必须加上UseCookiePolicy  否则手机qq浏览器访问不了cookie
        public static void UseIdentityServerCenterConnect(this IApplicationBuilder app)
        {
            app.UseCookiePolicy();
        }
    }
}

using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace IdentityServerCenterConnect.Authorization
{
    public class AppAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly UserGrpcService.User.UserClient userClient;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger<AppAuthorizationPolicyProvider> logger;

        public AppAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
            UserGrpcService.User.UserClient userClient,
            IMemoryCache memoryCache,
            ILogger<AppAuthorizationPolicyProvider> logger)
            : base(options)
        {
            this.userClient = userClient;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }



        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAssertion(async ctx =>
            {
                if (ctx.User?.Identity?.IsAuthenticated == true)
                {
                    var userId = ctx.User?.Claims.FirstOrDefault(e => e.Type == "sub")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        Google.Protobuf.Collections.RepeatedField<string> permissions = null;

                        permissions = await memoryCache.GetOrCreateAsync($"{userId}", async cache =>
                        {
                            cache.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                            try
                            {
                                //ctx.AbsoluteExpirationRelativeToNow = IdentityServerCenterConnectServiceCollectionExtensions.UserPermissionCacheTimeSpan;
                                var result = (await userClient.GetUserPermissonsAsync(
                                    new UserGrpcService.UserIdReq
                                    {
                                        UserId = userId
                                    })).Items;

                                return result;
                            }
                            catch(RpcException ex)
                            {
                                logger.LogError($"Grpc获取用户权限失败:{ex.Message}");
                            }
                            return null;
                        });

                        return permissions.Any(e => e == policyName);
                    }
                }
                return false;
            });

            return Task.FromResult(policy.Build());

            //如果在ConfigService AddAuthorization
            //获取回落政策
            //https://docs.microsoft.com/zh-cn/aspnet/core/security/authorization/iauthorizationpolicyprovider?view=aspnetcore-3.1
            //return this.GetFallbackPolicyAsync();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UserGrpcService;

namespace IdentityServerCenterConnect
{
    public class HttpContextHelper : IHttpContextHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly User.UserClient userClient;

        public HttpContextHelper(IHttpContextAccessor httpContextAccessor, UserGrpcService.User.UserClient userClient)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userClient = userClient;
        }

        /// <summary>
        /// 是否认证
        /// </summary>
        public bool IsAuthenticated => httpContextAccessor.HttpContext.User?.Identity?.IsAuthenticated ?? false;

        /// <summary>
        /// 登录路径
        /// </summary>
        public string LoginUrl => "/Account/Login";

        /// <summary>
        /// 注销路径
        /// </summary>
        public string LogoutUrl => "/Account/Logout";

        /// <summary>
        /// 更新个人信息路径
        /// </summary>
        public string UpdatePersonalInfoUrl => IdentityServerCenterConnectServiceCollectionExtensions.ConfigurationOptions.AuthorityUrl + "/Account/UpdateInfo";

        /// <summary>
        /// 当前用户id
        /// </summary>
        public string UserId => httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(e => e.Type == "sub")?.Value;

        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName => httpContextAccessor.HttpContext.User?.Identity?.Name;

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public async Task<UserReply> GetCurUserAsync()
        {
            var id = UserId;
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            UserReply user = await userClient.FindByIdAsync(new UserIdReq { UserId = UserId });
            return user;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UserGrpcService;

namespace IdentityServerCenterConnect
{
    public interface IHttpContextHelper
    {
        string UserId { get; }

        string UserName { get; }

        /// <summary>
        /// 获取当前用户id
        /// </summary>
        /// <returns></returns>
        Task<UserReply> GetCurUserAsync();

        /// <summary>
        /// 是否登录
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 登录路径
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        /// 注销路径
        /// </summary>
        string LogoutUrl { get; }


        /// <summary>
        /// 更新个人信息路径
        /// </summary>
        string UpdatePersonalInfoUrl { get; }
    }
}

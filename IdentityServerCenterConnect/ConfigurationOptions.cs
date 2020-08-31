using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerCenterConnect
{
    public class ConfigurationOptions
    {
        /// <summary>
        /// GRPC客户端URL
        /// </summary>
        public string GRpcUrl { get; set; }

        /// <summary>
        /// IdentityServer4地址
        /// </summary>
        public string AuthorityUrl { get; set; }

        /// <summary>
        /// 客户端Id
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// 客户端密码
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// 作用域 "openid", "profile", "api1"
        /// </summary>
        public List<string> Scopes  { get; set; }
    }
}
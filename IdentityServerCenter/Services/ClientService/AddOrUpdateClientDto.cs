using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ClientService
{
    public class AddOrUpdateClientDto
    {
        /// <summary>
        /// Id 如果是空，则创建，否则更新
        /// </summary>
        public int? Id { get; set; }

        [Required(ErrorMessage = "{0}不允许为空")]
        [DisplayName("(必填)客户端Id")]
        public string ClientId { get; set; }


        [Required(ErrorMessage = "{0}不允许为空")]
        [DisplayName("(必填)客户端名称")]
        public string ClientName { get; set; }


        [DisplayName("描述")]
        public string Description { get; set; }


        /// <summary>
        /// 重定位uris
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        [DisplayName("(必填)重定位url列表（可以多个，多个用 ; 号分割，英文的; 分号）,如 http://localhost:5000/signin-oidc")]
        public string RedirectUris { get; set; }


        [Required(ErrorMessage = "注销uri列表不能为空")]
        [DisplayName("(必填)注销url列表（可以多个，多个用 ; 号分割，英文的; 分号）,如 http://localhost:5000/signout-callback-oidc")]
        public string PostLogoutRedirectUris { get; set; }


        /// <summary>
        /// 作用域
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        [DisplayName("(必填)作用域列表（可以多个，多个用 ; 号分割，英文的; 分号）")]
        public string Scopes { get; set; }

        /// <summary>
        /// 请求同意授权页面
        /// </summary>
        public bool RequireConsent { get; set; }

        public string FrontChannelLogoutUri  { get; set; }
    }
}

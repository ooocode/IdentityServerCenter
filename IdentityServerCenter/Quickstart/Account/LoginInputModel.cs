// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class LoginInputModel
    {
        [DisplayName("账号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Username { get; set; }

        [DisplayName("密码")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Password { get; set; }

        [DisplayName("记住登录")]
        public bool RememberLogin { get; set; }


        public string ReturnUrl { get; set; }


        /// <summary>
        /// 是否启动双因素验证
        /// </summary>
        public bool RequiresTwoFactor { get; set; }

        /// <summary>
        /// 双因素验证验证码
        /// </summary>
        public string TwoFactorCode { get; set; }

        /// <summary>
        /// 按钮 （登录 or 取消）
        /// </summary>
        public string Button  { get; set; }
    }
}
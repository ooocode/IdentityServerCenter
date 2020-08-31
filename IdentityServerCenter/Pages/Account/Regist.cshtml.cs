using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Utility;

namespace IdentityServerCenter.Pages.Account
{
    [AllowAnonymous]
    public class RegistModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmailSender emailSender;

        public RegistModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        }


        public string SucceededMessage { get; set; }

        public string ErrorMessage { get; set; }


        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
            {
                var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false);
                if(user == null)
                {
                    return NotFound();
                }

                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await userManager.ConfirmEmailAsync(user, token).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    SucceededMessage = "成功激活邮件账号，请记住您的账号和密码，现在可以回到登录页面进行登录";
                }
                else
                {
                    ErrorMessage = "无效的令牌";
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(RegistViewModel registViewModel)
        {
            if (registViewModel is null)
            {
                throw new ArgumentNullException(nameof(registViewModel));
            }

            if (!TryValidateModel(registViewModel))
            {
                return BadRequest(ModelState);
            }

            //await userManager.DeleteAsync(await userManager.FindByNameAsync(registViewModel.Email).ConfigureAwait(false)).ConfigureAwait(false);

            var user = await userManager.FindByEmailAsync(registViewModel.Email).ConfigureAwait(false);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = GuidEx.NewGuid().ToString(CultureInfo.CurrentCulture),
                    UserName = registViewModel.Email,
                    Email = registViewModel.Email,
                    Password = registViewModel.Password,
                    EmailConfirmed = false
                };

                var result = await userManager.CreateAsync(user, registViewModel.Password).ConfigureAwait(false);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, $"账号创建失败[{result.Errors.FirstOrDefault()?.Description}]");
                    return BadRequest(ModelState);
                }
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, $"账号创建失败,邮件已被注册，并且被确认，如果是忘记了密码可以重新找回哦");
                return BadRequest(ModelState);
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "token生成错误");
                return BadRequest(ModelState);
            }

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Page(
                        "/Account/Regist",
                        pageHandler: null,
                        values: new { userId = user.Id, token = token },
                        protocol: Request.Scheme);

            string body = $"请点击此地址完成学习坊的账号注册： {callbackUrl} <a href=\"{callbackUrl}\" target=\"_blank\">点击验证</a>";
            await emailSender.SendEmailAsync(new string[] { user.Email }, "学习坊账号注册", body).ConfigureAwait(false);
            return Content($"已经发送注册验证邮件到【{user.Email}】,请查收验证");
        }

        public class RegistViewModel
        {
            [EmailAddress(ErrorMessage = "无效的邮箱地址")]
            public string Email { get; set; }

            [Required(ErrorMessage = "密码不能为空")]
            [StringLength(30, MinimumLength = 6, ErrorMessage = "密码长度6--30位")]
            public string Password { get; set; }

            [Required(ErrorMessage = "确认密码不能为空")]
            [Compare(nameof(Password))]
            [StringLength(30, MinimumLength = 6, ErrorMessage = "确认密码长度6--30位")]
            public string ConfirmPassword { get; set; }
        }
    }
}

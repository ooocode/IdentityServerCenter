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
                    SucceededMessage = "�ɹ������ʼ��˺ţ����ס�����˺ź����룬���ڿ��Իص���¼ҳ����е�¼";
                }
                else
                {
                    ErrorMessage = "��Ч������";
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
                    ModelState.AddModelError(string.Empty, $"�˺Ŵ���ʧ��[{result.Errors.FirstOrDefault()?.Description}]");
                    return BadRequest(ModelState);
                }
            }

            if (user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, $"�˺Ŵ���ʧ��,�ʼ��ѱ�ע�ᣬ���ұ�ȷ�ϣ������������������������һ�Ŷ");
                return BadRequest(ModelState);
            }

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError(string.Empty, "token���ɴ���");
                return BadRequest(ModelState);
            }

            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var callbackUrl = Url.Page(
                        "/Account/Regist",
                        pageHandler: null,
                        values: new { userId = user.Id, token = token },
                        protocol: Request.Scheme);

            string body = $"�����˵�ַ���ѧϰ�����˺�ע�᣺ {callbackUrl} <a href=\"{callbackUrl}\" target=\"_blank\">�����֤</a>";
            await emailSender.SendEmailAsync(new string[] { user.Email }, "ѧϰ���˺�ע��", body).ConfigureAwait(false);
            return Content($"�Ѿ�����ע����֤�ʼ�����{user.Email}��,�������֤");
        }

        public class RegistViewModel
        {
            [EmailAddress(ErrorMessage = "��Ч�������ַ")]
            public string Email { get; set; }

            [Required(ErrorMessage = "���벻��Ϊ��")]
            [StringLength(30, MinimumLength = 6, ErrorMessage = "���볤��6--30λ")]
            public string Password { get; set; }

            [Required(ErrorMessage = "ȷ�����벻��Ϊ��")]
            [Compare(nameof(Password))]
            [StringLength(30, MinimumLength = 6, ErrorMessage = "ȷ�����볤��6--30λ")]
            public string ConfirmPassword { get; set; }
        }
    }
}

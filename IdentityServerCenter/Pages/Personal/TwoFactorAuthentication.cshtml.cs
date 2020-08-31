using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace IdentityServerCenter
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly UrlEncoder urlEncoder;

        public TwoFactorAuthenticationModel(UserManager<ApplicationUser> userManager,
            UrlEncoder urlEncoder)
        {
            this.userManager = userManager;
            this.urlEncoder = urlEncoder;
        }

        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant ();
        }

        public string Key { get; set; }
        public string ImageBase64 { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //生成用户唯一的验证 key
            var userId = User?.Claims?.FirstOrDefault(e => e.Type == "sub")?.Value;
            var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false);
            var authenticatorKey = await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await userManager.ResetAuthenticatorKeyAsync(user).ConfigureAwait(false);
                authenticatorKey = await userManager.GetAuthenticatorKeyAsync(user).ConfigureAwait(false);
            }

            Key = FormatKey(authenticatorKey);

            var uri = $"otpauth://totp/{urlEncoder.Encode("学习坊登录中心")}:{urlEncoder.Encode(user.UserName)}" +
                $"?secret={authenticatorKey}&issuer={urlEncoder.Encode("学习坊登录中心")}&digits=6";


            using QRCodeGenerator qrGenerator = new QRCodeGenerator();
            using QRCodeData qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);


            using Base64QRCode qrCode = new Base64QRCode(qrCodeData);
            var qrCodeImageAsBase64 = qrCode.GetGraphic(20);

            ImageBase64 = $"data:image/png;base64,{qrCodeImageAsBase64}";
            return Page();
        }

        //private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    }
}
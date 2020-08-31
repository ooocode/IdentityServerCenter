using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter
{
    public class LoginWith2faViewModel
    {
        [Required]
        public string Code  { get; set; }

        public string ReturnUrl { get; set; }
    }

    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> signInManager;

        [BindProperty]
        public LoginWith2faViewModel LoginWith2FaViewModel { get; set; } = new LoginWith2faViewModel();

        public LoginWith2faModel(SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException(message: $"Unable to load two-factor authentication user.");
            }

            LoginWith2FaViewModel.ReturnUrl = HttpContext.Request.QueryString.Value?.Replace("?returnUrl=",string.Empty, StringComparison.OrdinalIgnoreCase);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
                if (user == null)
                {
                    throw new InvalidOperationException($"Unable to load two-factor authentication user.");
                }

                var authenticatorCode = LoginWith2FaViewModel.Code.Replace(" ", string.Empty, StringComparison.Ordinal)
                    .Replace("-", string.Empty, StringComparison.Ordinal);

                var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, false,false)
                    .ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return Redirect(LoginWith2FaViewModel.ReturnUrl);
                }

                ModelState.AddModelError(string.Empty,"无效的验证码");
            }
            return Page();
        }
    }
}
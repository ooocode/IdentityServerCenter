using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServerCenter
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;

        public LoginModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            _schemeProvider = schemeProvider ?? throw new ArgumentNullException(nameof(schemeProvider));
            _events = events ?? throw new ArgumentNullException(nameof(events));
        }

 
        public LoginViewModel LoginViewModel { get; set; }


        public async Task<IActionResult> OnGetAsync(string returnUrl, string userName, string password)
        {
            //方便第三方直接输入账号密码登录
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                LoginViewModel login = new LoginViewModel { Username = userName, Password = password };
                return await OnPost(login).ConfigureAwait(false);
            }


            LoginViewModel = await BuildLoginViewModelAsync(returnUrl).ConfigureAwait(false);

            //第三方登录
            if (LoginViewModel.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                return RedirectToAction("Challenge", "External", new { provider = LoginViewModel.ExternalLoginScheme, returnUrl });
            }

            return Page();
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="LoginViewModel">登录视图模型</param>
        /// <returns></returns>
        public async Task<IActionResult> OnPost(LoginViewModel LoginViewModel)
        {
            if (LoginViewModel is null)
            {
                throw new ArgumentNullException(nameof(LoginViewModel));
            }

            if (!TryValidateModel(LoginViewModel))
            {
                return BadRequest(ModelState);
            }

            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(LoginViewModel.ReturnUrl).ConfigureAwait(false);

            var user = await _userManager.Users.FirstOrDefaultAsync(e => e.UserName == LoginViewModel.Username).ConfigureAwait(false);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "账号不存在");
                return BadRequest(ModelState);
            }

            //await _userManager.SetLockoutEnabledAsync(user, true).ConfigureAwait(false);

            //登录，如果连续多次失败则锁定账号一段时间
            var signInResult = await _signInManager.PasswordSignInAsync(user, LoginViewModel.Password,
                isPersistent: LoginViewModel.RememberLogin,lockoutOnFailure:true).ConfigureAwait(false);

            //请求双因素验证
            if (signInResult.RequiresTwoFactor)
            {
                var twoFactorAuthenticationUser = await _signInManager.GetTwoFactorAuthenticationUserAsync().ConfigureAwait(false);
                if (twoFactorAuthenticationUser != null)
                {
                    //账号密码登录过了，需要双因素验证
                    signInResult = await _signInManager.TwoFactorAuthenticatorSignInAsync(LoginViewModel.TwoFactorCode, false, false).ConfigureAwait(false);
                }

                if (!signInResult.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "需要启动双因素验证");
                    return BadRequest(ModelState);
                }
            }
            if (signInResult.IsLockedOut)
            {
                var v = user.LockoutEnd.Value - DateTimeOffset.Now;
                ModelState.AddModelError(string.Empty, $"登录失败次数过多，账号暂时被锁定,[{v.Days}天{v.Hours}时{v.Minutes}分{v.Seconds}秒] 后解除限制");
                return BadRequest(ModelState);
            }

            if (signInResult.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, $"此账号不允许登录，可能是没有激活邮件或者电话号码");
                return BadRequest(ModelState);
            }

            //如果登录成功
            if (signInResult.Succeeded)
            {
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, 
                    clientId: context?.Client?.ClientId)).ConfigureAwait(false);

                //外部应用登录，非local登录
                if (context != null)
                {
                    if (await _clientStore.IsPkceClientAsync(context?.Client?.ClientId).ConfigureAwait(false))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        //return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });

                        //return Content($"/Account/Redirect?RedirectUrl={LoginViewModel.ReturnUrl}");
                    }

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    return Content(LoginViewModel.ReturnUrl);
                }

                // request for a local page
                if (Url.IsLocalUrl(LoginViewModel.ReturnUrl))
                {
                    return Content(LoginViewModel.ReturnUrl);
                }
                else if (string.IsNullOrEmpty(LoginViewModel.ReturnUrl))
                {
                    return Content("/");
                }
                else
                {
                    // user might have clicked on a malicious link - should be logged
                    throw new Exception("invalid return URL");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "账号或者密码错误");
                return BadRequest(ModelState);
            }
        }


        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="LoginViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostCancelAsync([FromBody]LoginViewModel LoginViewModel)
        {
            if (LoginViewModel is null)
            {
                throw new ArgumentNullException(nameof(LoginViewModel));
            }

            var context = await _interaction.GetAuthorizationContextAsync(LoginViewModel.ReturnUrl).ConfigureAwait(false);

            // the user clicked the "cancel" button

            //如果是外部应用则直接回去
            if (context != null)
            {
                //// if the user cancels, send a result back into IdentityServer as if they 
                //// denied the consent (even if this client does not require consent).
                //// this will send back an access denied OIDC error response to the client.
                //await _interaction.GrantConsentAsync(context, ConsentResponse.Denied).ConfigureAwait(false);
                await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied, "").ConfigureAwait(false);
                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                if (await _clientStore.IsPkceClientAsync(context?.Client?.ClientId).ConfigureAwait(false))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    //return RedirectToAction("Redirect", new RedirectViewModel { RedirectUrl = LoginViewModel.ReturnUrl });
                    //return Content($"/Account/Redirect?RedirectUrl={LoginViewModel.ReturnUrl}");

                    return Content(LoginViewModel.ReturnUrl);
                }

                return Content(LoginViewModel.ReturnUrl);
            }
            else
            {
                // since we don't have a valid context, then we just go back to the home page
                return Content("/");
            }
        }


        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl).ConfigureAwait(false);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP).ConfigureAwait(false) != null)
            {
                var local = context.IdP == IdentityServer4.IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync().ConfigureAwait(false);

            var providers = schemes
                .Where(x => x.DisplayName != null ||
                            (x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client?.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context?.Client?.ClientId).ConfigureAwait(false);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = AccountOptions.AllowRememberLogin,
                EnableLocalLogin = allowLocal && AccountOptions.AllowLocalLogin,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }
    }
}
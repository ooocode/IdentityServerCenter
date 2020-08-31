using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly IIdentityServerInteractionService identityServer;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IClientStore clientStore;

        public LogoutModel(IIdentityServerInteractionService identityServer,
            SignInManager<ApplicationUser> signInManager,
            IClientStore clientStore)
        {
            this.identityServer = identityServer ?? throw new ArgumentNullException(nameof(identityServer));
            this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            this.clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
        }


        public string SignOutIFrameUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            var ctx = await identityServer.GetLogoutContextAsync(logoutId).ConfigureAwait(false);

            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync().ConfigureAwait(false);
            }

            if (!string.IsNullOrEmpty(ctx?.PostLogoutRedirectUri))
            {
                return Redirect(ctx?.PostLogoutRedirectUri);
            }


            if (!string.IsNullOrEmpty(ctx?.SignOutIFrameUrl))
            {
                SignOutIFrameUrl = ctx?.SignOutIFrameUrl;
                return Page();
            }

            //本程序注销，不是外部程序发来的
            return Redirect("/");
        }
    }
}

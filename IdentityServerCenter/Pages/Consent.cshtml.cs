using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter.Pages
{
    [AllowAnonymous]
    public class ConsentModel : PageModel
    {
        private readonly IIdentityServerInteractionService identityServer;
        private readonly IResourceStore resourceStore;

        public ConsentModel(IIdentityServerInteractionService identityServer, IResourceStore resourceStore)
        {
            this.identityServer = identityServer;
            this.resourceStore = resourceStore;
        }

        public AuthorizationRequest AuthorizationRequest { get; set; }

        public ConsentInputViewModel ViewModel { get; set; }

        public IdentityServer4.Models.Resources Resources { get; set; }


        public async Task<IActionResult> OnGetAsync(string returnUrl)
        {
            AuthorizationRequest = await identityServer.GetAuthorizationContextAsync(returnUrl).ConfigureAwait(false);
            if (AuthorizationRequest == null)
            {
                return Forbid();
            }

            ViewModel = new ConsentInputViewModel
            {
                ReturnUrl = returnUrl,
                RememberConsent = true
            };

            var scopes = AuthorizationRequest.ValidatedResources.ParsedScopes.Select(e => e.ParsedName);
            Resources = await resourceStore.FindEnabledResourcesByScopeAsync(scopes)
                .ConfigureAwait(false);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(ConsentInputViewModel ViewModel)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            if (!TryValidateModel(ViewModel))
            {
                return BadRequest(ModelState);
            }

            var ctx = await identityServer.GetAuthorizationContextAsync(ViewModel.ReturnUrl).ConfigureAwait(false);
            if (ctx == null)
            {
                return Forbid();
            }


            ConsentResponse response = new ConsentResponse();
            if (ViewModel.OkOrCancelButton?.Equals("≤ª‘ –Ì", StringComparison.Ordinal) == true)
            {
                response.Error = AuthorizationError.AccessDenied;            }
            else
            {
                //if (ctx.Client.AllowOfflineAccess)
                //{
                //    ViewModel.Scopes.Add(IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess);
                //}
                response.RememberConsent = ViewModel.RememberConsent;
                response.ScopesValuesConsented = ViewModel.Scopes;
            }

            var userId = User.Claims.FirstOrDefault(e => e.Type == "sub")?.Value;

            //–¥»Îcookie
            await identityServer.GrantConsentAsync(ctx, response, userId).ConfigureAwait(false);

            return Redirect(ViewModel.ReturnUrl);
        }


        public class ConsentInputViewModel
        {
            [Required]
            public string ReturnUrl { get; set; }


            public List<string> Scopes { get; set; } = new List<string>();


            public bool RememberConsent { get; set; }


            public string OkOrCancelButton { get; set; }
        }
    }
}

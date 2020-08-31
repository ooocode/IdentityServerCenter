using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerCenter.Services.ClientService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter.Pages.ClientsManager.ClientPages
{
    public class AddOrUpdateClientModel : PageModel
    {
        private readonly IClientService clientService;

        public AddOrUpdateClientModel(IClientService clientService)
        {
            this.clientService = clientService;
        }

        public AddOrUpdateClientDto AddOrUpdateClientDto { get; set; }

        [FromRoute(Name = "Id")]
        public int? Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id.HasValue && Id > 0)
            {
                var client = await clientService.GetClientByIdAsync(Id.Value).ConfigureAwait(false);
                if (client == null)
                {
                    return NotFound();
                }

                //AddOrUpdateClientDto = new AddOrUpdateClientDto
                //{
                //    Id = client.Id,
                //    ClientId = client.ClientId,
                //    ClientName = client.ClientName,
                //    Description = client.Description,
                //    PostLogoutRedirectUris = client.PostLogoutRedirectUris,
                //    RedirectUris = client.RedirectUris,
                //    Scopes = client.Scopes,
                //    RequireConsent = client.RequireConsent,
                //    FrontChannelLogoutUri = client.FrontChannelLogoutUri
                //};
            }
            else
            {
                AddOrUpdateClientDto = new AddOrUpdateClientDto();
            }

            return Page();
        }


        /// <summary>
        /// 添加或更新客户端
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(AddOrUpdateClientDto AddOrUpdateClientDto)
        {
            if (!TryValidateModel(AddOrUpdateClientDto))
            {
                return BadRequest(ModelState);
            }

            //var result = await clientService.AddOrUpdateClientAsync(AddOrUpdateClientDto).ConfigureAwait(false);
            //if (result.Succeeded)
            //{
            //    return new OkResult();
            //}

            //ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}
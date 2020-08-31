using System.Threading.Tasks;
using IdentityServerCenter.Services.ClientService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter.Pages.ClientsManager.ClientPages
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly IClientService clientService;
   

        public IndexModel(IClientService clientService)
        {
            this.clientService = clientService;
        }


        public async Task<IActionResult> OnGetClientsAsync()
        {

            //var clients = await clientService.GetClientsAsync().ConfigureAwait(false);
            //return new JsonResult(clients);
            return new OkResult();
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteClientAsync(int id)
        {
            var result = await clientService.DeleteClientAsync(id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return RedirectToPage();
            }
            return NotFound();
        }
    }
}
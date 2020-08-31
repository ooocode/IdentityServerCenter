using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerCenter.Pages.IdentityResourcePage
{
    
  

    public class IndexModel : PageModel
    {
        private readonly ConfigurationDbContext configurationDbContext;

        public IndexModel(ConfigurationDbContext configurationDbContext)
        {
            this.configurationDbContext = configurationDbContext;
        }



 

        /// <summary>
        /// 删除身份资源
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteIdentityResourceAsync(int resourceId)
        {
            var resource = await configurationDbContext.IdentityResources.FirstOrDefaultAsync(e => e.Id == resourceId).ConfigureAwait(false);
            if(resource == null)
            {
                return NotFound();
            }

            configurationDbContext.IdentityResources.Remove(resource);

            var rows = await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
            return RedirectToPage();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using IdentityServerCenter.Data;
using IdentityServerCenter.Models;
using Microsoft.EntityFrameworkCore;
using Utility;

namespace IdentityServerCenter.Pages.UserPage.PermissionsPages
{
    public class CreateOrUpdateModel : PageModel
    {
        private readonly IdentityServerCenter.Data.ApplicationDbContext _context;

        public CreateOrUpdateModel(IdentityServerCenter.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [FromRoute]
        public string Id { get; set; }

        public Permisson AddOrUpdatePermissonViewModel { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                AddOrUpdatePermissonViewModel = await _context.Permissons.FirstOrDefaultAsync(m => m.Id == Id).ConfigureAwait(false);

                if (AddOrUpdatePermissonViewModel == null)
                {
                    return NotFound();
                }
            }
            else
            {
                AddOrUpdatePermissonViewModel = new Permisson() { Enabled = true };
            }

            return Page();
        }



        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(Permisson permisson)
        {
            if (permisson is null)
            {
                throw new ArgumentNullException(nameof(permisson));
            }

            if (!TryValidateModel(permisson))
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(permisson.Id))
            {
                var exist = await _context.Permissons.AnyAsync(e => e.Name == permisson.Name).ConfigureAwait(false);
                if (exist)
                {
                    ModelState.AddModelError(string.Empty, "权限名称已存在");
                    return BadRequest(ModelState);
                }
                //新建
                permisson.Id = GuidEx.NewGuid().ToString();
                _context.Permissons.Add(permisson);
            }
            else
            {
                _context.Attach(permisson).State = EntityState.Modified;
            }

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return BadRequest(ModelState);
            }

            return Content(permisson.Id);
        }
    }
}

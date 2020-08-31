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
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Database.Dtos.DictionaryDtos;

namespace IdentityServerCenter.Pages.UserPage.DictionaryPages
{
    public class CreateOrUpdateDictionaryTypeModel : PageModel
    {
        private readonly IDictionaryService dictionaryService;

        public CreateOrUpdateDictionaryTypeModel(IDictionaryService dictionaryService)
        {
            this.dictionaryService = dictionaryService ?? throw new ArgumentNullException(nameof(dictionaryService));
        }


        [FromRoute]
        public long? Id { get; set; }

        public CreateOrUpdateDictionaryTypeDto ViewModel { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if (Id.HasValue)
            {
                var dictionaryType = await dictionaryService.GetDictionaryTypeByIdAsync(Id.Value).ConfigureAwait(false);

                if (dictionaryType == null)
                {
                    return NotFound();
                }

                ViewModel = new CreateOrUpdateDictionaryTypeDto
                {
                    Id = dictionaryType.Id,
                    Name = dictionaryType.Name,
                    Remark = dictionaryType.Remark
                };
            }
            else
            {
                ViewModel = new CreateOrUpdateDictionaryTypeDto();
            }

            return Page();
        }



        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(CreateOrUpdateDictionaryTypeDto ViewModel)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            if (!TryValidateModel(ViewModel))
            {
                return BadRequest(ModelState);
            }

            var result = await dictionaryService.CreateOrUpdateDictionaryTypeAsync(ViewModel).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return new OkResult();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}

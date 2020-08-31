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
    public class CreateOrUpdateDictionaryModel : PageModel
    {
        private readonly IDictionaryService dictionaryService;

        public CreateOrUpdateDictionaryModel(IDictionaryService dictionaryService)
        {
            this.dictionaryService = dictionaryService ?? throw new ArgumentNullException(nameof(dictionaryService));
        }


        [FromRoute]
        public long? Id { get; set; }

        public CreateOrUpdateDictionaryDto ViewModel { get; set; }


        public async Task<IActionResult> OnGetAsync()
        {
            if (Id.HasValue)
            {
                var dictionary = await dictionaryService.GetDictionaryByIdAsync(Id.Value).ConfigureAwait(false);

                if (dictionary == null)
                {
                    return NotFound();
                }

                ViewModel = new CreateOrUpdateDictionaryDto
                {
                    DictionaryTypeId = dictionary.DictionaryTypeId,
                    Code = dictionary.Code,
                    Enabled = dictionary.Enabled,
                    Id = dictionary.Id,
                    Remark = dictionary.Remark,
                    Value = dictionary.Value
                };
            }
            else
            {
                ViewModel = new CreateOrUpdateDictionaryDto { Enabled = true };
            }

            return Page();
        }



        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(CreateOrUpdateDictionaryDto ViewModel)
        {
            if (ViewModel is null)
            {
                throw new ArgumentNullException(nameof(ViewModel));
            }

            if (!TryValidateModel(ViewModel))
            {
                return BadRequest(ModelState);
            }

            var result = await dictionaryService.CreateOrUpdateDictionaryAsync(ViewModel).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return new OkResult();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}

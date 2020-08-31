using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using IdentityServerCenter.Data;
using IdentityServerCenter.Models;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;

namespace IdentityServerCenter.Pages.UserPage.DictionaryPages
{
    public class DeletePermissonViewModel
    {
        public long DictionaryId { get; set; }
    }

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly IDictionaryService dictionaryService;

        public IndexModel(IDictionaryService dictionaryService)
        {
            this.dictionaryService = dictionaryService;
        }


        [FromRoute]
        public long? TypeId  { get; set; }


        private IEnumerable<dynamic> MakeVideModel(List<Database.Dtos.DictionaryDtos.DictionaryDto> dictionaries)
        {
            foreach (var item in dictionaries)
            {
                yield return new
                {
                    Id = item.Id.ToString(),
                    item.Code,
                    item.DictionaryTypeId,
                    item.Enabled,
                    item.Remark,
                    item.Value
                };
            }
        }

        /// <summary>
        /// 获取词典表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetDictionariesAsync()
        {
            List<Database.Dtos.DictionaryDtos.DictionaryDto> dictionaries = await dictionaryService.QueryDictionariesAsync(TypeId).ConfigureAwait(false);
            var result = MakeVideModel(dictionaries);
            return new JsonResult(result);
        }


        /// <summary>
        /// 删除词典数据
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteDictionaryAsync([FromBody] DeletePermissonViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!TryValidateModel(viewModel))
            {
                return BadRequest(ModelState);
            }

            var result = await dictionaryService.DeleteDictionaryByIdAsync(viewModel.DictionaryId).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return new OkResult();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}

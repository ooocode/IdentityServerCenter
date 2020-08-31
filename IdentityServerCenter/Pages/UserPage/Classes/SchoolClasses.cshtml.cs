using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerCenter.Data;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Study.Website.Pages.Teacher.Classes
{
    [IgnoreAntiforgeryToken]
    public class SchoolClassesModel : PageModel
    {
        private readonly ApplicationDbContext applicationDbContext;

        public SchoolClassesModel(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }
        public void OnGet()
        {

        }

        /// <summary>
        /// 从第一页开始
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetLoadSchoolClassesAsync(string name, int? pageIndex)
        {
            IQueryable<SchoolClass> schoolClass = applicationDbContext.SchoolClasses;
            if (!string.IsNullOrEmpty(name))
            {
                schoolClass = schoolClass.Where(e => e.Name.Contains(name));
            }

            if (!pageIndex.HasValue || pageIndex.Value <= 0)
            {
                pageIndex = 1;
            }

            var total = await schoolClass.CountAsync().ConfigureAwait(false);
            int take = 10;
            var rows = await schoolClass.Skip((pageIndex.Value - 1) * take)
                .Take(take)
                .ToListAsync()
                .ConfigureAwait(false);

            return new JsonResult(new
            {
                Total = total,
                Rows = rows,
                PageSize = take
            });
        }
    }
}
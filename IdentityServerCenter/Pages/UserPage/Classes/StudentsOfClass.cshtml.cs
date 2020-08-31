using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerCenter.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Study.Website.Pages.Teacher.Classes
{
    public class StudentsOfClassModel : PageModel
    {
        private readonly ApplicationDbContext applicationDbContext;

        public StudentsOfClassModel(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;
        }

        /// <summary>
        /// 班级id
        /// </summary>
        [FromRoute]
        public string ClassId  { get; set; }

        /// <summary>
        /// 根据班级id加载学生
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetLoadStudentsAsync()
        {
            var students = await applicationDbContext.Users
                .Where(e=>e.ClassId == ClassId)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            return new JsonResult(students);
        }
    }
}
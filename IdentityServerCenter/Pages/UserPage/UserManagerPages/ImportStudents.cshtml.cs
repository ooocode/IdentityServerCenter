using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Utility;
namespace Study.Website.Pages.Teacher
{
    public static class StringExtend
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }

    public class ImportStudentsModel : PageModel
    {
        const string pattern = "^[0-9]*$";
        private readonly ApplicationDbContext applicationDbContext;

        public static bool IsNumber(string str)
        {
            Regex rx = new Regex(pattern);
            return rx.IsMatch(str); //bool类型
        }

        public ImportStudentsModel(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }


        /// <summary>
        /// 添加学生
        /// </summary>
        /// <param name="upload"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(IFormFile upload)
        {
            if (upload != null/* && upload.Length <= (5 * 1024 * 1024)*/)
            {
                var extend = System.IO.Path.GetExtension(upload.FileName);
                string[] enableExtend = new string[] { ".xls", ".xlsx" };
                if (!enableExtend.Contains(extend.ToLower()))
                {
                    ModelState.AddModelError(string.Empty, "只能上传.xls 和 .xlsx 文件");
                    return Page();
                }

                //name  id
                //    Dictionary<string, string> tempClasses = new Dictionary<string, string>();
                using MemoryStream memoryStream = new MemoryStream();
                await upload.CopyToAsync(memoryStream).ConfigureAwait(false);

                //name  id
                Dictionary<string, string> tempClasses = new Dictionary<string, string>();

                var studentRole = await applicationDbContext.Roles.FirstOrDefaultAsync(e => e.Name == "学生").ConfigureAwait(false);

                if (studentRole == null)
                {
                    var role = new ApplicationRole("学生");

                    await applicationDbContext.Roles.AddAsync(role).ConfigureAwait(false);
                    await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                    studentRole = await applicationDbContext.Roles.FirstOrDefaultAsync(e => e.Name == "学生").ConfigureAwait(false);
                }

                if (studentRole == null)
                {
                    ModelState.AddModelError("", "没有学生角色");
                    return BadRequest(ModelState);
                }


                using (ExcelPackage package = new ExcelPackage(memoryStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int ColCount = worksheet.Dimension.Columns;


                    //坑 第一行居然不是从第0行开始
                    for (int row = 1; row <= rowCount; row++)
                    {
                        string className = worksheet.Cells[row, 1]?.Value?.ToString().Trim();
                        string userName = worksheet.Cells[row, 2]?.Value?.ToString().Trim();
                        string name = worksheet.Cells[row, 3]?.Value?.ToString().Trim();
                        string password = "123456";
                        if (className.IsNullOrEmpty() || userName.IsNullOrEmpty() || name.IsNullOrEmpty())
                        {
                            continue;
                        }

                        if (IsNumber(userName))
                        {
                            string classId = null;

                            var classInfo = await applicationDbContext.SchoolClasses.
                                FirstOrDefaultAsync(e => e.Name == className).ConfigureAwait(false);

                            if (classInfo == null)
                            {
                                if (!tempClasses.ContainsKey(className))
                                {
                                    tempClasses.Add(className, Guid.NewGuid().ToString("N"));
                                }

                                classId = tempClasses[className];
                            }
                            else
                            {
                                classId = classInfo.Id;
                            }

                            if (!await applicationDbContext.Users.AnyAsync(e => e.UserName == userName).ConfigureAwait(false))
                            {
                                var userId = GuidEx.NewGuid().ToString();
                                await applicationDbContext.Users.AddAsync(new IdentityServerCenter.Models.ApplicationUser
                                {
                                    Id = userId,
                                    UserName = userName,
                                    Name = name,
                                    NormalizedUserName = userName.ToUpper(CultureInfo.CurrentCulture),
                                    Password = password,
                                    ClassId = classId
                                }).ConfigureAwait(false);

                                //分配角色
                                await applicationDbContext.UserRoles.AddAsync(new IdentityUserRole<string>
                                {
                                    UserId = userId,
                                    RoleId = studentRole.Id
                                }).ConfigureAwait(false);
                            }
                        }
                    }

                    //添加班级
                    foreach (var c in tempClasses)
                    {
                        applicationDbContext.SchoolClasses.Add(new IdentityServerCenter.Models.SchoolClass { Id = c.Value, Name = c.Key });
                    }
                    var rows = await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                }


                return RedirectToPage();
            }

            ModelState.AddModelError(string.Empty, "空文件");

            return Page();
        }
    }
}
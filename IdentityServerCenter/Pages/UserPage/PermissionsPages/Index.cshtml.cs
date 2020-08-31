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

namespace IdentityServerCenter.Pages.UserPage.PermissionsPages
{
    public class PermissonViewModel
    {
        public string Id { get; set; }

        /// <summary>
        /// 名称（只能是数字或者字母）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplyName { get; set; }

        public string Desc { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// 分配到的角色
        /// </summary>
        public List<ApplicationRole> Roles { get; set; }
    }

    public class DeletePermissonViewModel
    {
        public string PermissonId  { get; set; }
    }

    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;


        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根据权限id获取对应角色列表
        /// </summary>
        /// <param name="permissonId"></param>
        /// <returns></returns>
        private async IAsyncEnumerable<ApplicationRole> getRolesByPermissionIdAsync(string permissonId)
        {
            var roleIds = await _context.RolePermissons
                  .Where(e => e.PermissonId == permissonId)
                  .Select(e => e.RoleId).ToListAsync().ConfigureAwait(false);

            foreach (var roleId in roleIds)
            {
                ApplicationRole roleInfo = await _context.Roles
                    .FirstOrDefaultAsync(e => e.Id == roleId).ConfigureAwait(false);

                yield return roleInfo;
            }
        }

        /// <summary>
        /// 获取权限表
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnGetPermissonsAsync()
        {
            List<PermissonViewModel> claims = new List<PermissonViewModel>();

            IQueryable<Permisson> Permissons = _context.Permissons;

            var result = await Permissons.OrderBy(e => e.Name).ToListAsync().ConfigureAwait(false);
            foreach (var Permisson in result)
            {
                PermissonViewModel model = new PermissonViewModel
                {
                    Id = Permisson.Id,
                    Desc = Permisson.Desc,
                    DisplyName = Permisson.DisplyName,
                    Enabled = Permisson.Enabled,
                    Name = Permisson.Name,
                    Roles = new List<ApplicationRole>()
                };


                await foreach (var item in getRolesByPermissionIdAsync(Permisson.Id))
                {
                    model.Roles.Add(item);
                }

                claims.Add(model);
            }

            return new JsonResult(claims);
        }


        /// <summary>
        /// 获取代码
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCodeStringAsync()
        {
            var Permissons = await _context.Permissons.ToListAsync().ConfigureAwait(false);

            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.AppendLine("namespace Permissons");
            str.AppendLine("{");
            str.AppendLine("\t/// <summary>");
            str.AppendLine("\t/// 权限字符串常量");
            str.AppendLine("\t/// </summary>");

            str.AppendLine("\tpublic static class Permisson");
            str.AppendLine("\t{");
            foreach (var item in Permissons)
            {
                str.AppendLine("\t\t/// <summary>");
                str.AppendLine($"\t\t/// {item.DisplyName}");
                if (!string.IsNullOrEmpty(item.Desc))
                {
                    str.AppendLine($"\t\t/// 描述：{item.Desc}");
                }

                str.AppendLine("\t\t/// </summary>");
                string constString = $"\t\tpublic const string {item.Name.Replace(".", "_", StringComparison.Ordinal)} = \"{item.Name }\";";
                str.AppendLine(constString);
                str.AppendLine();
            }
            str.AppendLine("\t}");
            str.AppendLine("}");
            return str.ToString();
        }

        /// <summary>
        /// 获取c# 代码文件
        /// </summary>
        public async Task<IActionResult> OnGetDownloadCsharpCodeAsync()
        {
            var code = await GetCodeStringAsync().ConfigureAwait(false);
            return File(System.Text.Encoding.UTF8.GetBytes(code), "text/plain", "Permission.cs");
        }


        public async Task<IActionResult> OnGetCsharpCodeAsync()
        {
            var code = await GetCodeStringAsync().ConfigureAwait(false);
            return Content(code, "text/plain", System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeletePermissonAsync([FromBody]DeletePermissonViewModel viewModel)
        {
            if (!TryValidateModel(viewModel))
            {
                return BadRequest(ModelState);
            }


            var permisson = await _context.Permissons.FirstOrDefaultAsync(e => e.Id == viewModel.PermissonId).ConfigureAwait(false);

            if (permisson == null)
            {
                ModelState.AddModelError(string.Empty, "权限不存在");
                return BadRequest(ModelState);
            }

            //先删除角色声明表关联的数据
            var roleClaims = _context.RolePermissons.Where(e => e.PermissonId == permisson.Id);
            _context.RolePermissons.RemoveRange(roleClaims);

            _context.Permissons.Remove(permisson);

            try
            {
                await _context.SaveChangesAsync().ConfigureAwait(false);
                return new OkResult();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}

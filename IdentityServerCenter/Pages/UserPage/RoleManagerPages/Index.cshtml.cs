using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityServerCenter.Pages.UserPage.RoleManagerPages
{
    [IgnoreAntiforgeryToken]
    public class IndexModel : PageModel
    {
        private readonly IRoleService roleService;

        public IndexModel(IRoleService roleService)
        {
            this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        /// <summary>
        /// 加载角色
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetLoadRolesAsync(int skip, int take, string search)
        {
            PaginationResult<ApplicationRole> result = await roleService.GetRolesAsync(skip, take, search).ConfigureAwait(false);
            return new JsonResult(result);
        }


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="deleteRoleViewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteRoleAsync([FromBody] DeleteRoleViewModel deleteRoleViewModel)
        {
            if (deleteRoleViewModel is null)
            {
                throw new ArgumentNullException(nameof(deleteRoleViewModel));
            }

            if (!TryValidateModel(deleteRoleViewModel))
            {
                return BadRequest(ModelState);
            }

            var role = await roleService.FindByIdAsync(deleteRoleViewModel.RoleId).ConfigureAwait(false);
            if(role == null)
            {
                return NotFound();
            }

            if (!role.Name.Equals(deleteRoleViewModel.RoleName.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(string.Empty, "角色输入不正确");
                return BadRequest(ModelState);
            }


            var result = await roleService.DeleteRoleAsync(deleteRoleViewModel.RoleId).ConfigureAwait(false);
            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }
    }

    /// <summary>
    /// 删除角色视图模型
    /// </summary>
    public class DeleteRoleViewModel
    {
        [Required(ErrorMessage = "Id不能为空")]
        public string RoleId { get; set; }


        [Required(ErrorMessage = "角色名称不能为空")]
        public string RoleName { get; set; }
    }
}
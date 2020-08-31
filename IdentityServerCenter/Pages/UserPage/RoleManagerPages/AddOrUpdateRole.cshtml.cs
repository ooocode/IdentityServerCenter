using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Pages.UserPage.RoleManagerPages
{
    public class AddOrUpdateRoleModel : PageModel
    {
        public AddOrUpdateRoleModel(ApplicationDbContext applicationDbContext,IRoleService roleService)
        {
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        /// <summary>
        /// 角色Id
        /// </summary>
        [FromRoute(Name = "Id")]
        public string Id { get; set; }

        private readonly ApplicationDbContext applicationDbContext;
        private readonly IRoleService roleService;

        public ApplicationRole Role { get; set; }

        /// <summary>
        /// 添加或更新角色视图模型
        /// </summary>
        public ApplicationRole CreateOrUpdateRoleDto { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!string.IsNullOrEmpty(Id))
            {
                Role = await roleService.FindByIdAsync(Id).ConfigureAwait(false);
                if (Role == null)
                {
                    return NotFound();
                }

                //默认角色禁止编辑删除
                //if (role.IsSystemDefaultRole)
                //{
                //    return Forbid();
                //}

                CreateOrUpdateRoleDto = new ApplicationRole
                {
                    Id = Role.Id,
                    Name = Role.Name,
                    Desc = Role.Desc
                };
            }
            else
            {
                CreateOrUpdateRoleDto = new ApplicationRole();
            }

            return Page();
        }


        /// <summary>
        /// 添加或者创建角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAddOrUpdateRoleAsync(ApplicationRole dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            if (!TryValidateModel(dto))
            {
                return BadRequest(ModelState);
            }

            var result = await roleService.CreateOrUpdateRoleAsync(dto).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }


        /// <summary>
        /// 分配角色权限
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAddRolePemissonsAsync([FromBody] AddRoleClaimsViewModel viewModel)
        {
            if (viewModel is null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (!TryValidateModel(viewModel))
            {
                return BadRequest(ModelState);
            }

            viewModel.Permissons = viewModel.Permissons.Distinct().ToList();

            //清空rolePermissions 即可
            var rolePermissons = applicationDbContext.RolePermissons
                .Where(e => e.RoleId == viewModel.RoleId);

            applicationDbContext.RolePermissons.RemoveRange(rolePermissons);


            //再添加到rolePermissions表
            foreach (var permissionId in viewModel.Permissons)
            {
                await applicationDbContext.RolePermissons.AddAsync(new RolePermisson
                {
                    RoleId = viewModel.RoleId,
                    PermissonId = permissionId
                }).ConfigureAwait(false);
            }

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ModelState.AddModelError(string.Empty, ex.InnerException?.Message ?? ex.Message);
                return BadRequest(ModelState);
            }

            return new OkResult();
        }
    }

    public class AddRoleClaimsViewModel
    {
        public string RoleId { get; set; }

        public List<string> Permissons { get; set; }
    }
}

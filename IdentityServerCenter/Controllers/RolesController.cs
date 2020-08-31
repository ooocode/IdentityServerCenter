using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServerCenter.Database.Dtos.RoleServiceDtos;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServerCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }
        /// <summary>
        /// 查询角色列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(typeof(PaginationResult<ApplicationRole>))]
        public async Task<IActionResult> GetRolesAsync(int skip, int take, string search)
        {
            PaginationResult<ApplicationRole> result = await roleService.GetRolesAsync(skip, take, search).ConfigureAwait(false);
            return new JsonResult(result);
        }

        /// <summary>
        /// 通过id获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [SwaggerResponse(typeof(ApplicationRole))]
        public async Task<IActionResult> GetRoleByIdAsync(string id)
        {
            var result = await roleService.FindByIdAsync(id).ConfigureAwait(false);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                return new JsonResult(result);
            }
        }

        /// <summary>
        /// 创建或者更新角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        //[HttpPost]
        //[SwaggerResponse(typeof(string))]
        //public async Task<IActionResult> CreateOrUpdateRoleAsync(CreateOrUpdateRoleDto dto)
        //{
        //    var result = await roleService.CreateOrUpdateRoleAsync(dto).ConfigureAwait(false);
        //    if (result.Succeeded)
        //    {
        //        return new JsonResult(result.Data);
        //    }

        //    ModelState.AddModelError(string.Empty, result.ErrorMessage);
        //    return BadRequest(ModelState);
        //}

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoleByIdAsync(string id)
        {
            var result = await roleService.DeleteRoleAsync(id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 获取角色拥有的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("{roleId}/permissons")]
        public async Task<List<Permisson>> GetRolePermissonsAsync(string roleId)
        {
            List<Permisson> result = await roleService.GetRolePermissonsAsync(roleId).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 重新分配权限到角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("{roleId}/permissons")]
        public async Task<IActionResult> ReAddPermissonsToRoleAsync(string roleId, ReAddPermissonsToRoleDto dto)
        {
            if (roleId != dto.RoleId)
            {
                ModelState.AddModelError(string.Empty, "roleId参数不一致");
            }
            var result = await roleService.ReAddPermissonsToRoleAsync(dto).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}

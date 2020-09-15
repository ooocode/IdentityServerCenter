using System;
using System.Threading.Tasks;
using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ManagerCenter.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissonsController : ControllerBase
    {
        private readonly IPermissonService permissonService;

        public PermissonsController(IPermissonService permissonService)
        {
            this.permissonService = permissonService ?? throw new ArgumentNullException(nameof(permissonService));
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PaginationResult<Permisson>> GetPermissonsAsync(int? skip = null, int? take = null, string search = null)
        {
            var ls = await permissonService.GetPermissonsAsync(skip,take,search).ConfigureAwait(false);
            return ls;
        }

        /// <summary>
        /// 根据id获取权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Permisson> GetPermissonByIdAsync(string id)
        {
            var permisson = await permissonService.GetPermissonByIdAsync(id).ConfigureAwait(false);
            return permisson;
        }

        /// <summary>
        /// 创建或者更新权限
        /// </summary>
        /// <param name="permisson"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdatePermissonAsync(Permisson permisson)
        {
            var result = await permissonService.CreateOrUpdatePermissonAsync(permisson).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermissonAsync(string id)
        {
            var result = await permissonService.DeletePermissonAsync(id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}

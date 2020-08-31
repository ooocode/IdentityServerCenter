using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Services.ApiResourceService;
using IdentityServerCenter.Services.IdentityResourceService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServerCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiScopesController : ControllerBase
    {
        private readonly IApiScopeService  apiScopeService;

        public ApiScopesController(IApiScopeService apiScopeService)
        {
            this.apiScopeService = apiScopeService ?? throw new ArgumentNullException(nameof(apiScopeService));
        }

        /// <summary>
        /// 获取api作用域列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<PaginationResult<ApiScopeDto>> GetApiScopesAsync(int? skip, int? take, string search)
        {
            return apiScopeService.GetApiScopesAsync(skip, take, search);
        }

        /// <summary>
        /// 根据id获取api作用域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Task<ApiScopeDto> GetApiScopeByIdAsync(int id)
        {
            return apiScopeService.GetApiScopeByIdAsync(id);
        }

        /// <summary>
        /// 创建或者更新api作用域
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateApiScopeAsync(ApiScopeDto dto)
        {
            var result = await apiScopeService.CreateOrUpdateApiScopeAsync(dto).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 删除api作用域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiScopeAsync(int id)
        {
            var result = await apiScopeService.DeleteApiScopeAsync(id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }
    }
}

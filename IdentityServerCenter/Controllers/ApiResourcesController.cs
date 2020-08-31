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
    public class ApiResourcesController : ControllerBase
    {
        private readonly IApiResourceService apiResourceService;

        public ApiResourcesController(IApiResourceService apiResourceService)
        {
            this.apiResourceService = apiResourceService ?? throw new ArgumentNullException(nameof(apiResourceService));
        }

        /// <summary>
        /// 获取身份资源列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<PaginationResult<ApiResourceDto>> GetApiResourcesAsync(int? skip, int? take, string search)
        {
            return apiResourceService.GetApiResourcesAsync(skip, take, search);
        }

        /// <summary>
        /// 根据id获取身份资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Task<ApiResourceDto> GetApiResourceByIdAsync(int id)
        {
            return apiResourceService.GetApiResourceByIdAsync(id);
        }

        /// <summary>
        /// 创建或者更新身份资源
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateApiResourceAsync(ApiResourceDto dto)
        {
            var result = await apiResourceService.CreateOrUpdateApiResourceAsync(dto).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 删除api资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApiResourceAsync(int id)
        {
            var result = await apiResourceService.DeleteApiResourceAsync(id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }


        /// <summary>
        /// 推荐的用户声明
        /// </summary>
        /// <returns></returns>
        [HttpGet("RecommendUserClaims")]
        public IEnumerable<string> GetRecommendUserClaims()
        {
            var jswtClassInfo = typeof(IdentityModel.JwtClaimTypes);
            var fis = jswtClassInfo.GetFields();  // 注意，这里不能有任何选项，否则将无法获取到const常量
            foreach (var item in fis)
            {
                var value = item.GetRawConstantValue().ToString();
                yield return value;
            }

            string[] otherTypes = { "微信", "QQ", "微博", "个性签名" };
            foreach (var item in otherTypes)
            {
                yield return item;
            }
        }
    }
}

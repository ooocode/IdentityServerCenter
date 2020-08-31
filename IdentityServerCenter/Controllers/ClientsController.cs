using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Services.ClientService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServerCenter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService clientService;

        public ClientsController(IClientService clientService)
        {
            this.clientService = clientService;
        }


        /// <summary>
        /// 获取客户端列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        public Task<PaginationResult<ClientDto>> GetClientsAsync(int? skip, int? take, string search)
        {
            return clientService.GetClientsAsync(skip, take, search);
        }

        /// <summary>
        /// 通过id获取客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public Task<ClientDto> GetClientByIdAsync(int id)
        {
            return clientService.GetClientByIdAsync(id);
        }

        /// <summary>
        /// 通过id删除客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClientByIdAsync(int id)
        {
            var result = await clientService.DeleteClientAsync(id).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 创建或者更新客户端
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateOrUpdateClientAsync(ClientDto dto)
        {
            var result = await clientService.CreateOrUpdateClientAsync(dto).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return Ok();
            }
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return BadRequest(ModelState);
        }

        /// <summary>
        /// 默认授权类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("ExampleGrantTypes")]
        public List<ExampleGrantType> GetExampleGrantTypes()
        {
            return clientService.GetExampleGrantTypes();
        }


        /// <summary>
        /// 可用的作用域
        /// </summary>
        /// <returns></returns>
        [HttpGet("AllEnabledScopes")]
        public Task<List<EnabledScopeDto>> GetAllEnabledScopesAsync()
        {
            return clientService.GetAllEnabledScopesAsync();
        }
    }
}

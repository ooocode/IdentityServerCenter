using IdentityServerCenter.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.ClientService
{
    public interface IClientService
    {
        /// <summary>
        /// 添加或者更新OIDC客户端
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Result> CreateOrUpdateClientAsync(ClientDto dto);

        /// <summary>
        /// 删除客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result> DeleteClientAsync(int id);


        Task<List<EnabledScopeDto>> GetAllEnabledScopesAsync();

        /// <summary>
        /// 根据主键id获取客户端
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ClientDto> GetClientByIdAsync(int id);

        ///// <summary>
        ///// 根据客户端Id获取客户端
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //Task<ClientDto> GetClientByClientAsync(string clientId);

        Task<PaginationResult<ClientDto>> GetClientsAsync(int? skip, int? take, string search);
        List<ExampleGrantType> GetExampleGrantTypes();
    }
}

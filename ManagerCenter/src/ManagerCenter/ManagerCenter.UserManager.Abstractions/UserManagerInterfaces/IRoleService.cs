using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Dtos.UserManagerDtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.Abstractions.Models.UserManagerModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions.UserManagerInterfaces
{
    public interface IRoleService
    {
        /// <summary>
        /// 创建或者更新角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        Task<DataResult<string>> CreateRoleAsync(CreateRoleDto role);

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="applicationRole"></param>
        /// <returns></returns>
        Task<DataResult<ApplicationRole>> UpdateRoleAsync(ApplicationRole applicationRole);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<Result> DeleteRolesAsync(List<ApplicationRole> roles);


        Task<ApplicationRole> FindByIdAsync(string roleId);

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        Task<List<Permisson>> GetRolePermissonsAsync(string roleId);

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<PaginationResult<ApplicationRole>> GetRolesAsync(int skip, int take, string search);


        Task<Result> ReAddPermissonsToRoleAsync(ReAddPermissonsToRoleDto dto);
    }
}

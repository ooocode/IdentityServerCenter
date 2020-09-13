using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions
{
    public interface IRoleService
    {
        Task<DataResult<string>> CreateOrUpdateRoleAsync(ApplicationRole role);
        Task<Result> DeleteRoleAsync(string roleId);
        Task<ApplicationRole> FindByIdAsync(string roleId);
        Task<List<Permisson>> GetRolePermissonsAsync(string roleId);
        Task<PaginationResult<ApplicationRole>> GetRolesAsync(int skip, int take, string search);
        Task<Result> ReAddPermissonsToRoleAsync(ReAddPermissonsToRoleDto dto);
    }
}

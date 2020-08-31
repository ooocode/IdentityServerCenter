using IdentityServerCenter.Database.Dtos.RoleServiceDtos;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerCenter.Database.Services
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

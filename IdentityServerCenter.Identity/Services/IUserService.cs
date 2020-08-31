using IdentityServerCenter.Database.Dtos.UserServiceDtos;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServerCenter.Database.Services
{
    public interface IUserService
    {
        Task<Result> AddToRolesAsync(AddToRolesDto dto);
        Task<DataResult<string>> CreateOrUpdateUserAsync(ApplicationUser user);
        Task<Result> CreateOrUpdateUserClaimAsync(ApplicationIdentityUserClaim claim);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result> DeleteUserClaimAsync(string userId, int cliamId);
        Task<ApplicationUser> FindByIdAsync(string id);
        Task<ApplicationUser> FindByUserNameAsync(string userName);
        Task<List<string>> GetPermissonsOfUserAsync(string userId);
        Task<List<ApplicationRole>> GetRolesOfUserByUserIdAsync(string userId);
        Task<List<ApplicationIdentityUserClaim>> GetUserClaimsAsync(string userId);
        Task<PaginationResult<ApplicationUser>> GetUsers(int skip, int take, string search);
    }
}

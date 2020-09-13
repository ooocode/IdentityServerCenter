using AutoMapper;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Dtos.UserServiceDtos;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Identity.Services;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace IdentityServerCenter.Database.Services.DefaultImpl
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<UserService> logger;
        private readonly ApplicationUserValidator applicationUserValidator;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper,
            ApplicationDbContext applicationDbContext,
            ILogger<UserService> logger,
            ApplicationUserValidator applicationUserValidator,
            IDataProtectionProvider )
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.applicationUserValidator = applicationUserValidator ?? throw new ArgumentNullException(nameof(applicationUserValidator));
        }

        /// <summary>
        /// 创建或者更新用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<DataResult<string>> CreateOrUpdateUserAsync(ApplicationUser user)
        {
            var v = await applicationUserValidator.ValidateAsync(user).ConfigureAwait(false);
            if (!v.IsValid)
            {
                return FailedDataResult<string>(v.Errors.FirstOrDefault().ErrorMessage);
            }


            //Id为空则创建
            IdentityResult result;
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = GuidEx.NewGuid().ToString();
                result = await userManager.CreateAsync(user, user.Password).ConfigureAwait(false);
            }
            else
            {
                result = await userManager.UpdateAsync(user).ConfigureAwait(false);
            }

            if (result.Succeeded)
            {
                return OkDataResult(user.Id);
            }

            return FailedDataResult<string>(result.Errors.FirstOrDefault().Description);
        }

        /// <summary>
        /// 通过Id查找用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            return user;
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByUserNameAsync(string userName)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.UserName == userName).ConfigureAwait(false);
            return user;
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PaginationResult<ApplicationUser>> GetUsers(int skip, int take, string search)
        {
            IQueryable<ApplicationUser> query = applicationDbContext.Users;

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();

                query = query.Where(e =>
                  e.UserName.Contains(search)
               || e.Name.Contains(search)
               || e.Email.Contains(search)
              );
            }

            skip = skip <= 0 ? 0 : skip;
            take = take <= 0 ? 10 : take;

            var total = await query.CountAsync().ConfigureAwait(false);
            var users = await query.Skip(skip).Take(take).ToListAsync().ConfigureAwait(false);

            return OkPaginationResult(users, total, take);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false);
            if (user == null)
            {
                return FailedResult("用户不存在");
            }

            var result = await userManager.DeleteAsync(user).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return FailedResult(result.Errors.FirstOrDefault().Description);
            }

            return OkResult();
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<Result> AddToRolesAsync(AddToRolesDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var existUser = await applicationDbContext.Users.AnyAsync(e => e.Id == dto.UserId).ConfigureAwait(false);
            if (!existUser)
            {
                return FailedResult("用户不存在");
            }

            //先删除
            var userRoles = applicationDbContext.UserRoles.Where(e => e.UserId == dto.UserId);
            applicationDbContext.UserRoles.RemoveRange(userRoles);

            foreach (var roleId in dto.RoleIds)
            {
                var existRole = await applicationDbContext.Roles.AnyAsync(e => e.Id == roleId).ConfigureAwait(false);
                if (!existRole)
                {
                    return FailedResult($"角色[{roleId}]不存在");
                }
                await applicationDbContext.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = dto.UserId, RoleId = roleId }).ConfigureAwait(false);
            }

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }

        /// <summary>
        /// 获取用户拥有的角色
        /// </summary>
        /// <returns></returns>
        public async Task<List<ApplicationRole>> GetRolesOfUserByUserIdAsync(string userId)
        {
            List<ApplicationRole> roles = await applicationDbContext.UserRoles
                .Where(e => e.UserId == userId).Select(e => e.RoleId) //先选出用户拥有的角色id
                .Join(applicationDbContext.Roles, e => e, ee => ee.Id, (roleId, role) => role).ToListAsync().ConfigureAwait(false);

            return roles;
        }

        /// <summary>
        /// 获取用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<ApplicationIdentityUserClaim>> GetUserClaimsAsync(string userId)
        {
            var result = await applicationDbContext.UserClaims.Where(e => e.UserId == userId).ToListAsync().ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// 创建或者更新用户声明
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result> CreateOrUpdateUserClaimAsync(ApplicationIdentityUserClaim claim)
        {
            if (claim.Id == 0)
            {
                await applicationDbContext.UserClaims.AddAsync(claim).ConfigureAwait(false);
            }
            else
            {
                applicationDbContext.UserClaims.Update(claim);
            }
            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch(DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }

      
        /// <summary>
        /// 删除用户声明
        /// </summary>
        /// <param name="cliamId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUserClaimAsync(string userId, int cliamId)
        {
            var claim = await applicationDbContext.UserClaims.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == cliamId).ConfigureAwait(false);
            if (claim == null)
            {
                return FailedResult($"{cliamId}不存在");
            }

            applicationDbContext.UserClaims.Remove(claim);
            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                var message = ex?.InnerException?.Message ?? ex.Message;
                logger.LogError(message);
                return FailedResult(message);
            }
        }

        /// <summary>
        /// 获取用户拥有的权限名称列表
        /// </summary>
        /// <param name="userId"></param>
        public async Task<List<string>> GetPermissonsOfUserAsync(string userId)
        {
            //查出该用户所有的角色id
            var userRoleIds = await applicationDbContext.UserRoles
                .Where(e => e.UserId == userId)
                .Select(e => e.RoleId).ToListAsync().ConfigureAwait(false);

            //角色权限表中包含权限id
            List<string> permissonIds = await applicationDbContext.RolePermissons
                .Where(e => userRoleIds.Contains(e.RoleId))
                .Distinct()
                .Select(e => e.PermissonId)
                .ToListAsync().ConfigureAwait(false);

            List<string> permissons = new List<string>();

            foreach (var permissonId in permissonIds.Distinct())
            {
                //获取声明类型信息
                var permisson = await applicationDbContext.Permissons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == permissonId && e.Enabled == true)
                    .ConfigureAwait(false);

                if (permisson != null)
                {
                    permissons.Add(permisson.Name);
                }
            }

            return permissons;
        }
    }
}

using AutoMapper;
using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Dtos.UserManagerDtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.Abstractions.Models.UserManagerModels;
using ManagerCenter.UserManager.Abstractions.UserManagerInterfaces;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.EntityFrameworkCore
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<UserService> logger;
        private readonly ITimeLimitedDataProtector dataProtector;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper,
            ApplicationDbContext applicationDbContext,
            ILogger<UserService> logger,
            IDataProtectionProvider dataProtectionProvider)
        {
            if (dataProtectionProvider is null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            dataProtector = dataProtectionProvider.CreateProtector(nameof(UserService)).ToTimeLimitedDataProtector();
        }


        private string EncryptUserId(string userId)
        {
            var encryptId = dataProtector.Protect(userId);
            return encryptId;
        }

        private ApplicationUser EncryptUserId(ApplicationUser user)
        {
            user.Id = dataProtector.Protect(user.Id);
            return user;
        }

        private string DecryptUserId(string encryptUserId)
        {
            var deCryptId = dataProtector.Unprotect(encryptUserId);
            return deCryptId;
        }

        private ApplicationUser DecryptUserId(ApplicationUser user)
        {
            user.Id = dataProtector.Unprotect(user.Id);
            return user;
        }


        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<DataResult<string>> CreateUserAsync(CreateUserDto dto)
        {
            var id = GuidEx.NewGuid().ToString();
            ApplicationUser user = new ApplicationUser
            {
                Id = id,
                UserName = dto.UserName,
                Name = dto.Name,
                Password = dto.Password
            };

            var identityResult = await userManager.CreateAsync(user, user.Password).ConfigureAwait(false);
            if (identityResult.Succeeded)
            {
                return OkDataResult(EncryptUserId(id));
            }

            return FailedDataResult<string>(identityResult.Errors.FirstOrDefault().Description);
        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<DataResult<string>> UpdateUserAsync(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user = DecryptUserId(user);

            //Id为空则创建
            IdentityResult identityResult = await userManager.UpdateAsync(user).ConfigureAwait(false);
            if (identityResult.Succeeded)
            {
                return OkDataResult(EncryptUserId(user.Id));
            }

            return FailedDataResult<string>(identityResult.Errors.FirstOrDefault().Description);
        }

        /// <summary>
        /// 通过Id查找用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            id = DecryptUserId(id);
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            return EncryptUserId(user);
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<ApplicationUser> FindByUserNameAsync(string userName)
        {
            var user = await applicationDbContext.Users.FirstOrDefaultAsync(e => e.UserName == userName).ConfigureAwait(false);
            user = EncryptUserId(user);
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
            users = users.Select(e => EncryptUserId(e)).ToList();

            return OkPaginationResult(users, total, take);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUsersAsync(List<ApplicationUser> users)
        {
            users = users.Select(e => DecryptUserId(e)).ToList();
            applicationDbContext.Users.RemoveRange(users);
            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }

            return OkResult();
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public async Task<Result> AddToRolesAsync(ApplicationUser user, List<string> roleIds)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (roleIds is null)
            {
                throw new ArgumentNullException(nameof(roleIds));
            }

            //先删除用户的角色
            var userRoles = applicationDbContext.UserRoles.Where(e => e.UserId == user.Id);
            applicationDbContext.UserRoles.RemoveRange(userRoles);

            foreach (var roleId in roleIds)
            {
                var existRole = await applicationDbContext.Roles.AnyAsync(e => e.Id == roleId).ConfigureAwait(false);
                if (!existRole)
                {
                    return FailedResult($"角色[{roleId}]不存在");
                }
                await applicationDbContext.UserRoles.AddAsync(new IdentityUserRole<string> { UserId = user.Id, RoleId = roleId }).ConfigureAwait(false);
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
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }


        /// <summary>
        /// 删除用户声明
        /// </summary>
        /// <param name="cliamId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUserClaimsAsync(List<ApplicationIdentityUserClaim> userClaims)
        {
            applicationDbContext.UserClaims.RemoveRange(userClaims);
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


        /// <summary>
        /// 用过id获取用户声明
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="claimId"></param>
        /// <returns></returns>
        public async Task<ApplicationIdentityUserClaim> GetUserClaimByIdAsync(string userId, int claimId)
        {
            var userClaim = await applicationDbContext.UserClaims.FirstOrDefaultAsync(e => e.UserId == userId && e.Id == claimId).ConfigureAwait(false);
            return userClaim;
        }
    }
}

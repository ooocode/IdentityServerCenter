using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Dtos.UserManagerDtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.Abstractions.Models.UserManagerModels;
using ManagerCenter.UserManager.Abstractions.UserManagerInterfaces;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.EntityFrameworkCore
{
    public class RoleService : ServiceBase, IRoleService
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext applicationDbContext;


        public RoleService(RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext applicationDbContext)
        {
            this.roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<DataResult<string>> CreateRoleAsync(CreateRoleDto role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            ApplicationRole applicationRole = new ApplicationRole
            {
                Id = GuidEx.NewGuid().ToString(),
                Name = role.Name,
                Desc = role.Desc
            };

            var identityResult =  await roleManager.CreateAsync(applicationRole).ConfigureAwait(false);
            if (identityResult.Succeeded)
            {
                return OkDataResult(applicationRole.Id);
            }

            return FailedDataResult<string>(identityResult.Errors.FirstOrDefault().Description);
        }



        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="applicationRole"></param>
        /// <returns></returns>
        public async Task<DataResult<ApplicationRole>> UpdateRoleAsync(ApplicationRole applicationRole)
        {
            var identityResult = await roleManager.UpdateAsync(applicationRole).ConfigureAwait(false);
            if (identityResult.Succeeded)
            {
                return OkDataResult(applicationRole);
            }

            return FailedDataResult<ApplicationRole>(identityResult.Errors.FirstOrDefault()?.Description);
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PaginationResult<ApplicationRole>> GetRolesAsync(int skip, int take, string search)
        {
            var query = applicationDbContext.Roles.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(e => e.Name.Contains(search) || e.Desc.Contains(search));
            }

            var tatal = await query.CountAsync().ConfigureAwait(false);
            List<ApplicationRole> rows = await query.Skip(skip).Take(take).AsNoTracking().ToListAsync().ConfigureAwait(false);
            return OkPaginationResult(rows, tatal, take);
        }


        /// <summary>
        /// 通过Id查询角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<ApplicationRole> FindByIdAsync(string roleId)
        {
            ApplicationRole role = await applicationDbContext.Roles.FirstOrDefaultAsync(e => e.Id == roleId).ConfigureAwait(false);
            return role;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<Result> DeleteRolesAsync(List<ApplicationRole> roles)
        {
            if (roles is null)
            {
                throw new ArgumentNullException(nameof(roles));
            }

            foreach (var role in roles)
            {
                if (role.NonEditable)
                {
                    return FailedResult("默认角色不可删除");
                }

                //先删除角色权限
                var rolePermissons = applicationDbContext.RolePermissons.Where(e => e.RoleId == role.Id);
                applicationDbContext.RolePermissons.RemoveRange(rolePermissons);

                //再删除角色
                applicationDbContext.Roles.Remove(role);
            }

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// 创建或更新角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>成功返回角色id</returns>
        public async Task<DataResult<string>> CreateOrUpdateRoleAsync(ApplicationRole role)
        {
            if (role is null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            IdentityResult result;
            if (string.IsNullOrEmpty(role.Id))
            {
                role.Id = GuidEx.NewGuid().ToString();
                //角色Id是空则创建
                result = await roleManager.CreateAsync(role).ConfigureAwait(false);
            }
            else
            {
                //更新
                result = await roleManager.UpdateAsync(role).ConfigureAwait(false);
            }
            if (result.Succeeded)
            {
                return OkDataResult(role.Id);
            }

            return FailedDataResult<string>(result.Errors.FirstOrDefault().Description);
        }



        /// <summary>
        /// 获取角色拥有的权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public async Task<List<Permisson>> GetRolePermissonsAsync(string roleId)
        {
            var permissons = await applicationDbContext.RolePermissons.Where(e => e.RoleId == roleId)
                .Join(applicationDbContext.Permissons, e => e.PermissonId, p => p.Id, (rolePermisson, permisson) => permisson)
                .ToListAsync().ConfigureAwait(false);

            return permissons;
        }

        /// <summary>
        /// 重新添加权限到角色
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result> ReAddPermissonsToRoleAsync(ReAddPermissonsToRoleDto dto)
        {
            if (dto is null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var existRole = await applicationDbContext.Roles.AnyAsync(e => e.Id == dto.RoleId).ConfigureAwait(false);
            if (!existRole)
            {
                return FailedResult("角色不存在");
            }

            dto.PermissonIds = dto.PermissonIds.Distinct().ToList();

            //先清空清空rolePermissions 即可
            var rolePermissons = applicationDbContext.RolePermissons
               .Where(e => e.RoleId == dto.RoleId);

            applicationDbContext.RolePermissons.RemoveRange(rolePermissons);


            //再添加到rolePermissions表
            foreach (var permissionId in dto.PermissonIds)
            {
                var existPermisson = await applicationDbContext.Permissons.AnyAsync(e => e.Id == permissionId).ConfigureAwait(false);
                if (!existPermisson)
                {
                    return FailedResult($"不存在权限id[{permissionId}]");
                }

                await applicationDbContext.RolePermissons.AddAsync(new RolePermisson
                {
                    RoleId = dto.RoleId,
                    PermissonId = permissionId
                }).ConfigureAwait(false);
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
    }
}

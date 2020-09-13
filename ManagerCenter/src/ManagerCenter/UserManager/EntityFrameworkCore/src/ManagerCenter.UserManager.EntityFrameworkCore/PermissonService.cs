using AutoMapper;
using ManagerCenter.Shared;
using ManagerCenter.UserManager.Abstractions;
using ManagerCenter.UserManager.Abstractions.Dtos;
using ManagerCenter.UserManager.Abstractions.Models;
using ManagerCenter.UserManager.EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.EntityFrameworkCore
{
    public class PermissonService: ServiceBase, IPermissonService
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IMapper mapper;

        public PermissonService(ApplicationDbContext applicationDbContext,IMapper mapper)
        {
            this.applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 获取所有权限
        /// </summary>
        /// <returns></returns>
        public async Task<PaginationResult<Permisson>> GetPermissonsAsync(int? skip, int? take, string search)
        {
            IQueryable<Permisson> permissons = applicationDbContext.Permissons;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                permissons = permissons.Where(e => e.Name.Contains(search) || e.DisplyName.Contains(search) || e.Desc.Contains(search));
            }

            int total = await permissons.CountAsync().ConfigureAwait(false);

            skip = skip ?? 0;
            take = take ?? 0;
            skip = skip < 0 ? 0 : skip;
            take = take <= 0 ? total : take;

            var ls = await permissons.AsNoTracking().Skip(skip.Value).Take(take.Value).ToListAsync().ConfigureAwait(false);
          
            return OkPaginationResult(ls, total, take.Value);
        }

        /// <summary>
        /// 根据id获取权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Permisson> GetPermissonByIdAsync(string id)
        {
            var permisson = await applicationDbContext.Permissons.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            return permisson;
        }

        /// <summary>
        /// 创建或者更新权限
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<DataResult<string>> CreateOrUpdatePermissonAsync(Permisson permisson)
        {
            if (permisson is null)
            {
                throw new ArgumentNullException(nameof(permisson));
            }

            if (!string.IsNullOrEmpty(permisson.Id))
            {
                applicationDbContext.Permissons.Update(permisson);
              
            }
            else
            {
                permisson.Id = GuidEx.NewGuid().ToString();
                await applicationDbContext.Permissons.AddAsync(permisson).ConfigureAwait(false);
            }

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkDataResult(permisson.Id);
            }
            catch (DbUpdateException ex)
            {
                return FailedDataResult<string>(ex);
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeletePermissonAsync(string id)
        {
            var permisson = await applicationDbContext.Permissons.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (permisson == null)
            {
                return FailedResult("权限未找到");
            }

            //先删除角色权限
            var rolePermissons = applicationDbContext.RolePermissons.Where(e => e.PermissonId == id);
            applicationDbContext.RolePermissons.RemoveRange(rolePermissons);

            applicationDbContext.Permissons.Remove(permisson);
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

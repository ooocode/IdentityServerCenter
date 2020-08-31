using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Identity.Services;
using IdentityServerCenter.Services.ApiResourceService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.IdentityResourceService
{
    /// <summary>
    /// Api作用域服务
    /// </summary>
    public class ApiScopeService : ServiceBase, IApiScopeService
    {
        private readonly ConfigurationDbContext configurationDbContext;
        private readonly IMapper mapper;

        public ApiScopeService(ConfigurationDbContext configurationDbContext, IMapper mapper)
        {
            this.configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IQueryable<ApiScope> ApiScopes => configurationDbContext.ApiScopes;

        /// <summary>
        /// 获取api资源
        /// </summary>
        /// <returns></returns>
        public async Task<PaginationResult<ApiScopeDto>> GetApiScopesAsync(int? skip, int? take, string search)
        {
            var query = ApiScopes;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(e => e.Name.Contains(search) || e.DisplayName.Contains(search) || e.Description.Contains(search));
            }

            int total = await query.CountAsync().ConfigureAwait(false);

            skip ??= 0;
            take ??= 10;

            List<ApiScope> resources = await query.Skip(skip.Value).Take(take.Value).ToListAsync().ConfigureAwait(false);
            var dtos = resources.Select(e => ApiScopeDto.FromDbEntity(mapper, e));

            return new PaginationResult<ApiScopeDto>
            {
                Succeeded = true,
                Total = total,
                Rows = dtos.ToList(),
                PageSize = take.Value
            };
        }

        /// <summary>
        /// 通过id获取api作用域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiScopeDto> GetApiScopeByIdAsync(int id)
        {
            var apiScope = await ApiScopes
                 .AsNoTracking()
                 .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (apiScope == null)
            {
                return null;
            }

            var dto = ApiScopeDto.FromDbEntity(mapper, apiScope);
            return dto;
        }

        /// <summary>
        /// 删除api作用域
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteApiScopeAsync(int id)
        {
            var apiScope = await configurationDbContext.ApiScopes
               .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (apiScope == null)
            {
                return FailedResult("作用域不存在");
            }

            configurationDbContext.ApiScopes.Remove(apiScope);
            try
            {
                await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }

        /// <summary>
        /// 创建或者更新api作用域
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result> CreateOrUpdateApiScopeAsync(ApiScopeDto dto)
        {
            var entity = dto.ToDbEntity();
            if (entity.Id == 0)
            {
                //创建
                await configurationDbContext.ApiScopes.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                var fromDbEntity = await ApiScopes.FirstOrDefaultAsync(e => e.Id == entity.Id).ConfigureAwait(false);
                if (fromDbEntity == null)
                {
                    return FailedResult("不存在api作用域");
                }

                fromDbEntity = mapper.Map(entity, fromDbEntity);
            }

            try
            {
                int rows = await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
                if (rows == 0)
                {
                    return FailedResult("已执行，但未更新数据库");
                }
                return OkResult();
            }
            catch (DbUpdateException ex)
            {
                return FailedResult(ex);
            }
        }
    }
}

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
    /// Api资源服务
    /// </summary>
    public class ApiResourceService : ServiceBase, IApiResourceService
    {
        private readonly ConfigurationDbContext configurationDbContext;
        private readonly IMapper mapper;

        public ApiResourceService(ConfigurationDbContext configurationDbContext, IMapper mapper)
        {
            this.configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IQueryable<ApiResource> ApiResources => configurationDbContext.ApiResources.Include(e =>e.Scopes);

        /// <summary>
        /// 获取api资源
        /// </summary>
        /// <returns></returns>
        public async Task<PaginationResult<ApiResourceDto>> GetApiResourcesAsync(int? skip, int? take, string search)
        {
            var query = ApiResources;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(e => e.Name.Contains(search) || e.DisplayName.Contains(search) || e.Description.Contains(search));
            }

            int total = await query.CountAsync().ConfigureAwait(false);

            skip ??= 0;
            take ??= 10;

            List<ApiResource> resources = await query.Skip(skip.Value).Take(take.Value).ToListAsync().ConfigureAwait(false);
            var dtos = mapper.Map<List<ApiResourceDto>>(resources);

            return new PaginationResult<ApiResourceDto>
            {
                Succeeded = true,
                Total = total,
                Rows = dtos,
                PageSize = take.Value
            };
        }

        /// <summary>
        /// 通过id获取api资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiResourceDto> GetApiResourceByIdAsync(int id)
        {
            ApiResource resource = await ApiResources
                 .AsNoTracking()
                 .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (resource == null)
            {
                return null;
            }

            var dto = mapper.Map<ApiResourceDto>(resource);
            return dto;
        }

        /// <summary>
        /// 删除api资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteApiResourceAsync(int id)
        {
            var resource = await configurationDbContext.ApiResources
               .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (resource == null)
            {
                return FailedResult("资源不存在");
            }

            configurationDbContext.ApiResources.Remove(resource);
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
        /// 创建或者更新资源
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result> CreateOrUpdateApiResourceAsync(ApiResourceDto dto)
        {
            IdentityServer4.Models.ApiResource model = mapper.Map<IdentityServer4.Models.ApiResource>(dto);
            

            var entity = model.ToEntity();
            if (!dto.Id.HasValue || dto.Id == 0)
            {
                //创建
                await configurationDbContext.ApiResources.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                entity.Id = dto.Id.Value;

                var fromDbEntity = await ApiResources.FirstOrDefaultAsync(e => e.Id == entity.Id).ConfigureAwait(false);
                if(fromDbEntity == null)
                {
                    return FailedResult("不存在身份资源");
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

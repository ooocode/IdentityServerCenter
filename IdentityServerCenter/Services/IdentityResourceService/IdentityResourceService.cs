using AutoMapper;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerCenter.Identity;
using IdentityServerCenter.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.IdentityResourceService
{
    public class IdentityResourceService : ServiceBase, IIdentityResourceService
    {
        private readonly ConfigurationDbContext configurationDbContext;
        private readonly IMapper mapper;

        public IdentityResourceService(ConfigurationDbContext configurationDbContext, IMapper mapper)
        {
            this.configurationDbContext = configurationDbContext ?? throw new ArgumentNullException(nameof(configurationDbContext));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        private IQueryable<IdentityResource> IdentityResources => configurationDbContext.IdentityResources.Include(e => e.UserClaims);

        /// <summary>
        /// 获取身份资源
        /// </summary>
        /// <returns></returns>
        public async Task<PaginationResult<IdentityResourceDto>> GetIdentityResourcesAsync(int? skip, int? take, string search)
        {
            var query = IdentityResources;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                query = query.Where(e => e.Name.Contains(search) || e.DisplayName.Contains(search) || e.Description.Contains(search));
            }

            int total = await query.CountAsync().ConfigureAwait(false);

            skip ??= 0;
            take ??= 10;

            IList<IdentityResourceDto> dtos = new List<IdentityResourceDto>();
            List<IdentityResource> resources = await query.Skip(skip.Value).Take(take.Value).ToListAsync().ConfigureAwait(false);
            foreach (var item in resources)
            {
                var model = item.ToModel();
                IdentityResourceDto dto = mapper.Map<IdentityResourceDto>(model);
                dto.Id = item.Id;
                dtos.Add(dto);
            }

            return new PaginationResult<IdentityResourceDto>
            {
                Succeeded = true,
                Total = total,
                Rows = dtos,
                PageSize = take.Value
            };
        }

        /// <summary>
        /// 通过id获取身份资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IdentityResourceDto> GetIdentityResourceByIdAsync(int id)
        {
            var resource = await IdentityResources
                 .AsNoTracking()
                 .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (resource == null)
            {
                return null;
            }

            var model = resource.ToModel();
            var dto = mapper.Map<IdentityResourceDto>(model);
            dto.Id = resource.Id;
            return dto;
        }

        /// <summary>
        /// 删除身份资源
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result> DeleteIdentityResourceAsync(int id)
        {
            var resource = await configurationDbContext.IdentityResources
               .FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
            if (resource == null)
            {
                return FailedResult("身份资源不存在");
            }

            configurationDbContext.IdentityResources.Remove(resource);
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
        public async Task<Result> CreateOrUpdateIdentityResourceAsync(IdentityResourceDto dto)
        {
            if(dto.UserClaims == null || dto.UserClaims.Count == 0)
            {
                return FailedResult("UserClaims不能为空值");
            }

            IdentityServer4.Models.IdentityResource model = mapper.Map<IdentityServer4.Models.IdentityResource>(dto);
            var entity = model.ToEntity();
            if (!dto.Id.HasValue || dto.Id == 0)
            {
                //创建
                await configurationDbContext.IdentityResources.AddAsync(entity).ConfigureAwait(false);
            }
            else
            {
                entity.Id = dto.Id.Value;

                var fromDbEntity = await IdentityResources.FirstOrDefaultAsync(e => e.Id == entity.Id).ConfigureAwait(false);
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

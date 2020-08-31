using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.Services.IdentityResourceService
{
    public class ApiScopeDto : IdentityServer4.Models.ApiScope
    {
        public int? Id { get; set; }


        public static ApiScopeDto FromDbEntity(IMapper mapper, ApiScope apiScopeEntity)
        {
            var model = apiScopeEntity.ToModel();
            ApiScopeDto apiScopeDto = mapper.Map<ApiScopeDto>(model);
            apiScopeDto.Id = apiScopeEntity.Id;
            return apiScopeDto;
        }

        public ApiScope ToDbEntity()
        {
            var entity = this.ToEntity();
            entity.Id = this.Id ?? 0;
            return entity;
        }
    }
}

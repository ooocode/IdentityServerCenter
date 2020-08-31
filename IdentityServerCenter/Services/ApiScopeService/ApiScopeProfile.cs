using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerCenter.Services.IdentityResourceService;

namespace IdentityServerCenter.Database.Services.Profiles
{
    public class ApiScopeProfile : Profile
    {
        public ApiScopeProfile()
        {
            CreateMap<IdentityServer4.Models.ApiScope, ApiScopeDto>().ReverseMap();
            CreateMap<ApiScope, ApiScope>().ReverseMap();
        }
    }
}

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerCenter.Services.IdentityResourceService;

namespace IdentityServerCenter.Database.Services.Profiles
{
    public class ApiResourceProfile : Profile
    {
        public ApiResourceProfile()
        {
            CreateMap<IdentityServer4.Models.ApiResource, ApiResourceDto>().ReverseMap();
            CreateMap<ApiResource, ApiResource>();
            CreateMap<ApiResource, ApiResourceDto>().ReverseMap();
            //CreateMap<IdentityServer4.EntityFramework.Entities.Client, IdentityServer4.EntityFramework.Entities.Client>().ReverseMap();
          
        }
    }
}

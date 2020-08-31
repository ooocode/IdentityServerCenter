using AutoMapper;
using IdentityServer4.EntityFramework.Entities;
using IdentityServerCenter.Services.IdentityResourceService;

namespace IdentityServerCenter.Database.Services.Profiles
{
    public class IdentityResourceProfile : Profile
    {
        public IdentityResourceProfile()
        {
            CreateMap<IdentityServer4.Models.IdentityResource, IdentityResourceDto>().ReverseMap();
            CreateMap<IdentityResource, IdentityResource>();
            //CreateMap<IdentityServer4.EntityFramework.Entities.Client, IdentityServer4.EntityFramework.Entities.Client>().ReverseMap();
          
        }
    }
}

using AutoMapper;
using IdentityServerCenter.Services.ClientService;

namespace IdentityServerCenter.Database.Services.Profiles
{
    public class ClientProfile : Profile
    {
        public ClientProfile()
        {
            CreateMap<IdentityServer4.Models.Client, ClientDto>().ReverseMap();

            CreateMap<IdentityServer4.EntityFramework.Entities.Client, IdentityServer4.EntityFramework.Entities.Client>().ReverseMap();
            //CreateMap<ApplicationRole, RoleListReplay.Types.RoleItem>()
            //    .ForMember(dest => dest.CreateDateTime,
            //        opt => opt.MapFrom(src => Timestamp.FromDateTimeOffset(src.CreateDateTime ?? DateTimeOffset.Now)))
            //    .ForMember(dest => dest.LastModifyDateTime,
            //        opt => opt.MapFrom(src => Timestamp.FromDateTimeOffset(src.LastModifyDateTime ?? DateTimeOffset.Now)))

            //  .ForAllOtherMembers(opt => opt.Condition((src, dest, obj) =>{  return obj!=null; }));
        }
    }
}

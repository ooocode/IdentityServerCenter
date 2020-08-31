using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using IdentityModel;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using UserGrpcService;

namespace IdentityServerCenter.Rpc
{

    public class ProfileEx : Profile
    {
        public ProfileEx()
        {
            CreateMap<ApplicationUser, UserReply>()
              .ForMember(dest => dest.LockoutEnd,
                 opt => opt.MapFrom(src => Timestamp.FromDateTimeOffset(src.LockoutEnd ?? DateTimeOffset.Now)))
             .ForAllOtherMembers(opt => opt.Condition((src, dest, obj) => { return obj != null; }));

            CreateMap<ApplicationRole, RoleListReplay.Types.RoleItem>()
              .ForAllOtherMembers(opt => opt.Condition((src, dest, obj) => { return obj != null; }));
        }
    }
}

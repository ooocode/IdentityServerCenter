using AutoMapper;
using AutoMapper.QueryableExtensions;
using Google.Protobuf.Collections;
using Grpc.Core;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using IdentityServerCenter.Models;
using IdentityServerCenter.Services.ClientService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Study.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserGrpcService;
using SchoolClass = IdentityServerCenter.Models.SchoolClass;

namespace IdentityServerCenter.Rpc
{
    public class ProfileServiceEx : UserGrpcService.User.UserBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly IClientService clientService;
        private readonly IMapper mapper;

        public ProfileServiceEx(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext applicationDbContext,
            IClientService clientService,
            IMapper mapper)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.applicationDbContext = applicationDbContext;
            this.clientService = clientService;
            this.mapper = mapper;
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserList> GetUsers(GetUsersReq request, ServerCallContext context)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IQueryable<ApplicationUser> query = applicationDbContext.Users;
            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(e => e.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                    || e.UserName.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                     || e.Email.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                     || e.PhoneNumber.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }
            var total = await query.CountAsync().ConfigureAwait(false);

            var users = (await query
                .Skip((int)request.Skip)
                .Take((int)request.Task)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false));

            UserList userList = new UserList();
            userList.Total = (uint)total;
            userList.Users.AddRange(mapper.Map<RepeatedField<UserReply>>(users));
            return userList;
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async override Task<RoleListReplay> GetRoles(Google.Protobuf.WellKnownTypes.Empty request, ServerCallContext context)
        {
            List<ApplicationRole> ls = await applicationDbContext.Roles
                .ToListAsync()
                .ConfigureAwait(false);

            RoleListReplay replay = new RoleListReplay();
            replay.Roles.AddRange(mapper.Map<RepeatedField<RoleListReplay.Types.RoleItem>>(ls));
            return replay;
        }

        /// <summary>
        /// 为用户分配角色
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<BoolReply> AddUserToRole(AddUserToRoleReq request, ServerCallContext context)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await applicationDbContext.Users.FirstOrDefaultAsync(e => e.Id == request.UserId).ConfigureAwait(false);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "用户未找到"));
            }

            var role = await applicationDbContext.Roles.FirstOrDefaultAsync(e => e.Id == request.RoleId).ConfigureAwait(false);
            if (role == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "用户未找到"));
            }

            var userRole = applicationDbContext.UserRoles.FirstAsync(e => e.UserId == request.UserId && e.RoleId == request.RoleId);
            if (userRole != null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "用户存在该角色"));
            }

            await applicationDbContext.UserRoles.AddAsync(new IdentityUserRole<string>
            {
                UserId = request.UserId,
                RoleId = request.RoleId
            }).ConfigureAwait(false);

            await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            return new BoolReply { Value = true };
        }

        /// <summary>
        /// 从用户角色表中移除用户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<BoolReply> RemoveUserFromUserRole(AddUserToRoleReq request, ServerCallContext context)
        {
            var userRole = await applicationDbContext.UserRoles.FirstAsync(e => e.UserId == request.UserId
                && e.RoleId == request.RoleId).ConfigureAwait(false);
            if (userRole == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "不存在记录"));
            }

            applicationDbContext.UserRoles.Remove(userRole);
            await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            return new BoolReply { Value = true };
        }

        /// <summary>
        /// 通过id查找用户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserReply> FindById(UserIdReq request, ServerCallContext context)
        {
            ApplicationUser user = await userManager.Users.FirstOrDefaultAsync(e => e.Id == request.UserId).ConfigureAwait(false);
            if(user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "用户不存在"));
            }

            //如果名字为空，可能是第三方账号登录  name保存在UserClaims表
            if (string.IsNullOrEmpty(user.Name))
            {
                //第三方登录提供商
                var extendProvider = await applicationDbContext.UserLogins
                    .FirstOrDefaultAsync(e => e.UserId == request.UserId)
                    .ConfigureAwait(false);
                if (extendProvider != null)
                {
                    user.Name = $"[{extendProvider.ProviderDisplayName}]" + (await applicationDbContext.UserClaims
                        .FirstOrDefaultAsync(e => e.UserId == request.UserId && e.ClaimType == "name")
                        .ConfigureAwait(false))?.ClaimValue;
                }
            }
            var userReply = mapper.Map<UserReply>(user);

            return userReply;
        }

        /// <summary>
        /// 通过用户名查找用户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserReply> FindByUserName(UserNameReq request, ServerCallContext context)
        {
            ApplicationUser user = await userManager.FindByNameAsync(request.UserName).ConfigureAwait(false);
            var userReply = mapper.Map<UserReply>(user);
            return userReply;
        }

        /// <summary>
        /// 通过id获取用户拥有的角色
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserRolesReply> GetRolesOfUser(UserIdReq request, ServerCallContext context)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.Id == request.UserId).ConfigureAwait(false);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "用户未找到"));
            }

            var roles = await userManager.GetRolesAsync(user).ConfigureAwait(false);

            UserRolesReply userRolesReply = new UserRolesReply();
            userRolesReply.Roles.AddRange(roles);
            return userRolesReply;
        }


        /// <summary>
        /// 获取角色下的用户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserList> GetUsersInRole(GetUsersInRoleReq request, ServerCallContext context)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var role = await roleManager.FindByIdAsync(request.RoleId.ToString()).ConfigureAwait(false);
            if (role == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "角色未找到"));
            }

            var query = applicationDbContext.UserRoles
                .Where(e => e.RoleId == role.Id)
                .Join(applicationDbContext.Users, e => e.UserId, (u) => u.Id, (a, b) => b);

            if (!string.IsNullOrEmpty(request.Query))
            {
                query = query.Where(e => e.Name.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                    || e.UserName.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                     || e.Email.Contains(request.Query, StringComparison.OrdinalIgnoreCase)
                     || e.PhoneNumber.Contains(request.Query, StringComparison.OrdinalIgnoreCase));
            }

            var total = await query.CountAsync().ConfigureAwait(false);

            var ls = await query.Skip((int)request.Skip)
             .Take((int)request.Task)
             .ToListAsync()
             .ConfigureAwait(false);

            UserList userList = new UserList();
            userList.Total = (uint)total;
            userList.Users.AddRange(mapper.Map<RepeatedField<UserReply>>(ls));
            return userList;
        }


        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<BoolReply> UpdateAwatar(UserAvatarReq request, ServerCallContext context)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var user = await userManager.Users.FirstOrDefaultAsync(e => e.Id == request.UserId).ConfigureAwait(false);
            if (user == null)
            {
                return null;
            }

            user.Photo = request.AvatarUrl;
            user.SecurityStamp = Guid.NewGuid().ToString();
            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);

            return new BoolReply { Value = result.Succeeded };
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<BoolReply> UpdatePassword(UserPasswordReq request, ServerCallContext context)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(e => e.Id == request.UserId).ConfigureAwait(false);
            if (user == null)
            {
                return null;
            }

            user.SecurityStamp = Guid.NewGuid().ToString();
            user.Password = request.NewPassword;
            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);

            return new BoolReply { Value = result.Succeeded };
        }


        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<BoolReply> UpdateUser(UpdateUserReq request, ServerCallContext context)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var user = await userManager.FindByIdAsync(request.UserId.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                return null;
            }

            user.SecurityStamp = Guid.NewGuid().ToString();

            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            user.Email = request.Email;
            user.Sex = byte.Parse(request.Sex.ToString());
            user.Desc = request.Desc;

            var result = await userManager.UpdateAsync(user).ConfigureAwait(false);

            return new BoolReply { Value = result.Succeeded };
        }

        /// <summary>
        /// 根据班级id获取用户列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserList> GetUsersByClassId(UserNameReq request, ServerCallContext context)
        {
            var users = await userManager.Users.Where(e => e.ClassId == request.UserName).ToListAsync().ConfigureAwait(false);
            UserList userList = new UserList();

            userList.Users.AddRange(users.Select(e => mapper.Map<UserReply>(e)));
            return userList;
        }

        /// <summary>
        /// 通过角色获取用户
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserList> GetUsersByRoleName(RoleNameReq request, ServerCallContext context)
        {
            IList<ApplicationUser> users = await userManager.GetUsersInRoleAsync(request.RoleName)
                .ConfigureAwait(false);

            var result = users.Skip((int)request.Skip).Take((int)request.Take);

            UserList userList = new UserList();
            userList.Users.AddRange(result.Select(e => mapper.Map<UserReply>(e)));
            return userList;
        }


        private SchoolClassReply SchoolClassToSchoolClassReply(SchoolClass schoolClass)
        {
            return new SchoolClassReply
            {
                Id = schoolClass.Id,
                Name = schoolClass.Name,
                Desc = schoolClass.Desc ?? string.Empty
            };
        }

        /// <summary>
        /// 获取学校班级
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<SchoolClassListReply> GetSchoolClasses(SchoolClassesReq request, ServerCallContext context)
        {
            if (request == null) throw new ArgumentNullException(nameof(GetSchoolClasses));

            SchoolClassListReply reply = new SchoolClassListReply();

            IQueryable<SchoolClass> clsses = applicationDbContext.SchoolClasses;
            if (!string.IsNullOrEmpty(request.QueryName))
            {
                clsses = clsses.Where(e => e.Name.Contains(request.QueryName.Trim(), StringComparison.OrdinalIgnoreCase));
            }
            var result = await clsses
                .Skip((int)request.Skip)
                .Take((int)request.Take)
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            reply.SchoolClasses.AddRange(result.Select(e => SchoolClassToSchoolClassReply(e)));

            //总数
            reply.Total = (uint)await clsses.CountAsync().ConfigureAwait(false);
            return reply;
        }


        /// <summary>
        /// 根据班级id获取班级信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserGrpcService.SchoolClassReply> GetSchoolClassById(IdReq request, ServerCallContext context)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var result = await applicationDbContext.SchoolClasses
                .FirstOrDefaultAsync(e => e.Id == request.Id)
                .ConfigureAwait(false);

            if (result == null)
            {
                context.Status = new Status(StatusCode.NotFound, "不存在的班级");
                return null;
            }
            else
            {
                return SchoolClassToSchoolClassReply(result);
            }
        }

        /// <summary>
        /// 根据班级名称获取班级信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<UserGrpcService.SchoolClassReply> GetSchoolClassByName(NameReq request, ServerCallContext context)
        {
            var result = await applicationDbContext.SchoolClasses
               .FirstOrDefaultAsync(e => e.Name == request.Name)
               .ConfigureAwait(false);

            if (result == null)
            {
                context.Status = new Status(StatusCode.NotFound, "不存在的班级");
                return null;
            }
            else
            {
                return SchoolClassToSchoolClassReply(result);
            }
        }


        /// <summary>
        /// 根据用户id返回用户的权限列表
        /// </summary>
        /// <param name="request">用户id </param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<StringListRes> GetUserPermissons(UserIdReq request, ServerCallContext context)
        {
            //查出该用户所有的角色id
            var userRoleIds = await applicationDbContext.UserRoles
                .Where(e => e.UserId == request.UserId)
                .Select(e => e.RoleId).ToListAsync().ConfigureAwait(false);

            //角色claim表中身份包含该权限id
            List<string> permissonIds = await applicationDbContext.RolePermissons
                .Where(e => userRoleIds.Contains(e.RoleId))
                .Distinct()
                .Select(e => e.PermissonId)
                .ToListAsync().ConfigureAwait(false);

            List<string> permissons = new List<string>();

            foreach (var id in permissonIds)
            {
                //获取声明类型信息
                var permisson = await applicationDbContext.Permissons
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == id && e.Enabled == true)
                    .ConfigureAwait(false);

                if (permisson != null)
                {
                    permissons.Add(permisson.Name);
                }
            }

            permissons = permissons.Distinct().ToList();

            StringListRes stringListRes = new StringListRes();
            stringListRes.Items.AddRange(permissons);
            return stringListRes;
        }
    }
}

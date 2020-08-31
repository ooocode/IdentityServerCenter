using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServerCenter.Database.Services;
using IdentityServerCenter.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerCenter
{
    public class ProfileServiceEx : DefaultProfileService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public ProfileServiceEx(UserManager<ApplicationUser> userManager, ILogger<ProfileServiceEx> logger,
            IUserService userService)
            : base(logger)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        //基本信息
        private async Task<List<Claim>> GetBaseInfoClaimsAsync(string userId)
        {
            List<Claim> claims = new List<Claim>();

            var user = await userService.FindByIdAsync(userId).ConfigureAwait(false);
            if (user != null)
            {
                foreach (System.Reflection.PropertyInfo item in user.GetType().GetProperties())
                {
                    var name = item.Name;
                    string[] ignoreProps = { nameof(user.Password) ,
                            nameof(user.PasswordHash),
                            nameof(user.ConcurrencyStamp),
                            nameof(user.SecurityStamp)
                        };

                    if (ignoreProps.Contains(name))
                    {
                        continue;
                    }

                    var value = item.GetValue(user);
                    if (value != null)
                    {
                        claims.Add(new Claim(name, value.ToString()));
                    }
                    else
                    {
                        claims.Add(new Claim(name, string.Empty));
                    }
                }
            }
            return claims;
        }


        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context).ConfigureAwait(false);

            List<Claim> claims = new List<Claim>();
            var id = context.Subject.GetSubjectId();
            //用户信息终结点，获取基本信息
            if (context.Caller == IdentityServer4.IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint)
            {
                var baseInfoClaims = await GetBaseInfoClaimsAsync(id).ConfigureAwait(false);
                claims.AddRange(baseInfoClaims);
            }

            //如果是访问令牌和用户信息终结点加入role
            if (context.Caller == IdentityServer4.IdentityServerConstants.ProfileDataCallers.ClaimsProviderAccessToken
                || context.Caller == IdentityServer4.IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint)
            {
                var roleClaims = context.Subject.Claims.Where(e => e.Type == JwtClaimTypes.Role);
                claims.AddRange(roleClaims);

                //用户信息终结点还加入permissons
                if (context.Caller == IdentityServer4.IdentityServerConstants.ProfileDataCallers.UserInfoEndpoint)
                {
                    var permissons = await userService.GetPermissonsOfUserAsync(id).ConfigureAwait(false);
                    foreach (var item in permissons)
                    {
                        claims.Add(new Claim("permissons", item));
                    }
                }
            }

            if (claims.Count > 0)
            {
                context.IssuedClaims.AddRange(claims);
            }
        }


        ///// <summary>
        ///// 是否激活
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //public async Task IsActiveAsync(IsActiveContext context)
        //{
        //    var userId = context.Subject?.Identity.GetSubjectId();
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        var user = await userManager.FindByIdAsync(userId).ConfigureAwait(false);
        //        if (user != null)
        //        {
        //            //不锁定
        //            context.IsActive = !(await userManager.IsLockedOutAsync(user).ConfigureAwait(false));
        //        }
        //    }
        //}



        //public ProfileServiceEx(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
        //    :base(userManager, claimsFactory)
        //{
        //    this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        //}

        //public ProfileServiceEx(UserManager<ApplicationUser> userManager,
        //    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, 
        //    ILogger<IdentityServer4.AspNetIdentity.ProfileService<ApplicationUser>> logger)
        //    :base(userManager, claimsFactory, logger)
        //{
        //    this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        //}

        //public override async Task IsActiveAsync(IsActiveContext context)
        //{
        //    if (context is null)
        //    {
        //        throw new ArgumentNullException(nameof(context));
        //    }

        //    var id = context.Subject.GetSubjectId();
        //    var user = await userManager.FindByIdAsync(id).ConfigureAwait(false);
        //    if (user != null)
        //    {
        //        //不锁定
        //        context.IsActive = !(await userManager.IsLockedOutAsync(user).ConfigureAwait(false));
        //    }
        //}
    }
}

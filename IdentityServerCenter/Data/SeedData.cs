using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerCenter.Data;
using IdentityServerCenter.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utility;

namespace IdentityServerCenter.Models
{
    public class SeedData
    {
        private readonly ConfigurationDbContext configurationDbContext;
        private readonly ApplicationDbContext applicationDbContext;
        private readonly UserManager<ApplicationUser> userManager;
        public const string defaultSuperAdminRole = "超级管理员";
        public const string defaultSuperAdminUserName = "Administrator";
        public const string defaultSuperAdminUserPassword = "Administrator";


        public SeedData(ConfigurationDbContext configurationDbContext,
            ApplicationDbContext applicationDbContext,
            UserManager<ApplicationUser> userManager)
        {
            this.configurationDbContext = configurationDbContext;
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
        }

        public async Task InitAsync()
        {
            await AddIdentityResourcesAsync().ConfigureAwait(false);
            await AddRolesAndUsersAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// 添加默认身份资源
        /// </summary>
        private async Task AddIdentityResourcesAsync()
        {
            IdentityResource OpenId = new IdentityServer4.Models.IdentityResources.OpenId() { DisplayName = "用户id" }.ToEntity();
            var Profile = new IdentityServer4.Models.IdentityResources.Profile()
            { DisplayName = "基本资料", Description = "包含姓名、性别、生日等信息" }.ToEntity();
            var Email = new IdentityServer4.Models.IdentityResources.Email() { DisplayName = "邮件" }.ToEntity();
            var Phone = new IdentityServer4.Models.IdentityResources.Phone() { DisplayName = "手机号码" }.ToEntity();
            var Address = new IdentityServer4.Models.IdentityResources.Address() { DisplayName = "家庭地址" }.ToEntity();
            IdentityResource[] identityResources = { OpenId, Profile, Email, Phone, Address };
            foreach (var identityResource in identityResources)
            {
                var exist = await configurationDbContext.IdentityResources.AnyAsync(e => e.Name == identityResource.Name).ConfigureAwait(false);
                if (!exist)
                {
                    await configurationDbContext.IdentityResources.AddAsync(identityResource).ConfigureAwait(false);
                }
            }

            await configurationDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        private async Task AddRolesAndUsersAsync()
        {
            //创建角色
            var existRole = await applicationDbContext.Roles.AnyAsync(e => e.Name == defaultSuperAdminRole).ConfigureAwait(false);
            if (!existRole)
            {
                var role = new ApplicationRole
                {
                    Id = GuidEx.NewGuid().ToString(),
                    Name = defaultSuperAdminRole,
                    NormalizedName = defaultSuperAdminRole.ToUpper(CultureInfo.CurrentCulture),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NonEditable = true, //默认角色不可编辑
                    Desc = "系统内置角色"
                };

                await applicationDbContext.Roles.AddAsync(role).ConfigureAwait(false);
            }

            var existAdmin = await applicationDbContext.Users.AnyAsync(e => e.UserName == defaultSuperAdminUserName).ConfigureAwait(false);
            if (!existAdmin)
            {
                var user = new ApplicationUser
                {
                    Id = GuidEx.NewGuid().ToString(),
                    UserName = defaultSuperAdminUserName,
                    NormalizedUserName = defaultSuperAdminUserName.ToUpper(CultureInfo.CurrentCulture),
                    Password = defaultSuperAdminUserPassword,
                    Name = defaultSuperAdminUserName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                await applicationDbContext.Users.AddAsync(user).ConfigureAwait(false);
            }

            await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);

            var admin = await userManager.Users.FirstOrDefaultAsync(e => e.UserName == defaultSuperAdminUserName).ConfigureAwait(false);
            if (admin != null)
            {
                await userManager.AddToRoleAsync(admin, defaultSuperAdminRole).ConfigureAwait(false);
            }
        }
    }
}

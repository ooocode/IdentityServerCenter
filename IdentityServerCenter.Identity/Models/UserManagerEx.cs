using IdentityServerCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerCenter.Models
{
    public class UserManagerEx : UserManager<ApplicationUser>
    {
        private readonly ApplicationDbContext applicationDbContext;

        public UserManagerEx(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<ApplicationUser>> logger,
            ApplicationDbContext applicationDbContext)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this.applicationDbContext = applicationDbContext;
        }

        /// <summary>
        /// 不支持userClaim存储在cookie里面，避免cookie过大
        /// </summary>
        public override bool SupportsUserClaim => true;

        /// <summary>
        /// 暂不支持安全戳
        /// </summary>
        public override bool SupportsUserSecurityStamp => false;

        /// <summary>
        /// 支持锁定账号
        /// </summary>
        public override bool SupportsUserLockout => true;

        public override async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return await Task.FromResult(user.Password == password).ConfigureAwait(false);
        }

        /// <summary>
        /// 添加用户声明
        /// </summary>
        /// <param name="user"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public override async Task<IdentityResult> AddClaimAsync(ApplicationUser user, Claim claim)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (claim is null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var entity = new Database.Models.ApplicationIdentityUserClaim(claim.Type, claim.Value, claim.ValueType)
            {
                UserId = user.Id
            };

            applicationDbContext.UserClaims.Add(entity);

            try
            {
                await applicationDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                return IdentityResult.Failed(new IdentityError { Description = ex.InnerException?.Message ?? ex.Message });
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// 获取用户ClaimTypes
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public override async Task<IList<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = await applicationDbContext.UserClaims.Where(e => e.UserId == user.Id).ToListAsync().ConfigureAwait(false);
            var result = claims?.Select(e => new Claim(e.ClaimType, e.ClaimValue, e.ClaimValueType))?.ToList();
            return result;
        }


        //public override async Task<string> GetSecurityStampAsync(ApplicationUser user)
        //{
        //    if (string.IsNullOrEmpty(user.SecurityStamp))
        //    {
        //        var u = await applicationDbContext.Users.FirstOrDefaultAsync(e => e.Id == user.Id);
        //        if (u != null)
        //        {
        //            u.SecurityStamp = Guid.NewGuid().ToString();
        //            await UpdateAsync(u).ConfigureAwait(false);
        //            return u.SecurityStamp;
        //        }
        //    }

        //    return user.SecurityStamp;
        //}
    }
}

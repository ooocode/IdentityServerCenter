using FluentValidation;
using IdentityServerCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerCenter.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string Photo { get; set; }

        /// <summary>
        /// 密码(以后再哈希保存吧)
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public byte Sex { get; set; }


        /// <summary>
        /// 个人简介
        /// </summary>
        public string Desc { get; set; }


        /// <summary>
        /// 班级id
        /// </summary>
        public string ClassId { get; set; }


        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
    }

    public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
    {
        public ApplicationUserValidator(ApplicationDbContext applicationDbContext)
        {
            RuleFor(x => x.UserName).NotEmpty().Length(1, 255).WithMessage("用户名错误");
            RuleFor(x => x.Password).NotEmpty().Length(6, 255).WithMessage("密码不能为空");
            RuleFor(x => x.Name).NotEmpty().Length(1, 255).WithMessage("显示名称不能为空");

            //当id是空值时，说明是创建，则用户名不能重复
            RuleFor(x => x.Id).MustAsync(async (user, id, ctx, can) =>
            {
                var existUser = await applicationDbContext.Users.AnyAsync(e => e.UserName == user.UserName, cancellationToken: can)
                .ConfigureAwait(false);
                return !existUser;
            }).When(x => string.IsNullOrEmpty(x.Id)).WithMessage("用户名已存在");
        }
    }
}

using FluentValidation;
using IdentityServerCenter.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace IdentityServerCenter.Database.Models
{
    public class ApplicationRole:IdentityRole
    {
        public ApplicationRole()
        {
           
        }

        public ApplicationRole(string roleName)
            :base(roleName)
        {
            
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc  { get; set; }

        /// <summary>
        /// 一旦创建了将不可编辑
        /// </summary>
        public bool NonEditable { get; set; }
    }



    public class ApplicationRoleValidator : AbstractValidator<ApplicationRole>
    {

        private readonly ApplicationDbContext applicationDbContext;


        public ApplicationRoleValidator(ApplicationDbContext applicationDbContext)
        {
            this.applicationDbContext = applicationDbContext;

            RuleFor(x => x.Name).NotEmpty().WithMessage("角色名称不能为空").Length(1, 255).WithMessage("角色名称长度为1-255位");
           

            //当id是空值时，说明是创建，则用户名不能重复
            RuleFor(x => x.Id).MustAsync(async (user, id, ctx, can) =>
            {
                var existUser = await applicationDbContext.Roles.AnyAsync(e => e.Name == user.Name.Trim(), cancellationToken: can).ConfigureAwait(false);
                return !existUser;
            }).When(x => string.IsNullOrEmpty(x.Id)).WithMessage("角色名称已经存在");


            //当id是空值时，说明是创建，则用户名不能重复
            RuleFor(x => x.Id).MustAsync(async (user, id, ctx, can) =>
            {
                var role = await applicationDbContext.Roles.FirstOrDefaultAsync(e => e.Id == id, cancellationToken: can).ConfigureAwait(false);
                if(role == null || role.NonEditable)
                {
                    return false;
                }

                return true;
            }).When(x => !string.IsNullOrEmpty(x.Id)).WithMessage("角色名称已经存在");
        }
    }
}

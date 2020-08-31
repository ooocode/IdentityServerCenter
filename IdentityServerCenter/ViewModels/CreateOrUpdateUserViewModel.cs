using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerCenter.ViewModels
{
    public class CreateOrUpdateUserViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }
    }


    public class CreateOrUpdateUserViewModelValidator : AbstractValidator<CreateOrUpdateUserViewModel>
    {
        public CreateOrUpdateUserViewModelValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().Length(1, 255).WithMessage("用户名错误");
            RuleFor(x => x.Password).NotEmpty().Length(6, 255).WithMessage("密码不能为空");
            RuleFor(x => x.Name).NotEmpty().Length(1, 255).WithMessage("显示名称不能为空");
        }
    }
}

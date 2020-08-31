using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerCenter.Database.Models
{
    public class AppIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError 
            {
                 Code = nameof(DuplicateUserName),
                 Description = $"用户名{userName}重复，此用户名称已经被注册"
            };
        }

        public override IdentityError ConcurrencyFailure()
        {
            return new IdentityError
            {
                Code = nameof(ConcurrencyFailure),
                Description = $"并发操作异常，数据已经被更改"
            };
        }
    }
}

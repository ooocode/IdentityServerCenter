using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ManagerCenter.UserManager.Abstractions.Models
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
}

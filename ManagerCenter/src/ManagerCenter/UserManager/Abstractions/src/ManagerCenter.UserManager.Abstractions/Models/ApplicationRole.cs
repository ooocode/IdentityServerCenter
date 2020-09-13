using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace ManagerCenter.UserManager.Abstractions.Models
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
}

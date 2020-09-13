using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Models
{
    /// <summary>
    /// 角色权限
    /// </summary>
    public class RolePermisson
    {
        [Key]
        public string RoleId  { get; set; }

        [Key]
        public string PermissonId  { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Dtos.UserManagerDtos
{
    public class ReAddPermissonsToRoleDto
    {
        /// <summary>
        /// 角色id
        /// </summary>
        [Required(ErrorMessage = "角色Id不能为空")]
        public string RoleId { get; set; }

        /// <summary>
        /// 权限id列表
        /// </summary>
#pragma warning disable CA2227 // 集合属性应为只读
        public List<string> PermissonIds { get; set; }
#pragma warning restore CA2227 // 集合属性应为只读
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityServerCenter.Database.Dtos.RoleServiceDtos
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
        public List<string> PermissonIds { get; set; }
    }
}

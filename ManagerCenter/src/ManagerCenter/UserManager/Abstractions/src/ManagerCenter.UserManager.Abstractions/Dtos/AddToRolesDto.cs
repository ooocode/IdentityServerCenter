using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Dtos
{
    public class AddToRolesDto
    {
        [Required]
        public string UserId  { get; set; }


        [Required]
#pragma warning disable CA2227 // 集合属性应为只读
        public List<string> RoleIds  { get; set; }
#pragma warning restore CA2227 // 集合属性应为只读
    }
}

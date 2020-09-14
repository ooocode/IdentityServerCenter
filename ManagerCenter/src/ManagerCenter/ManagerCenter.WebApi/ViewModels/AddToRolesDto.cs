using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ManagerCenter.WebApi.ViewModels
{
    /// <summary>
    /// 为用户分配角色视图模型
    /// </summary>
    public class AddToRolesViewModel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public string UserId  { get; set; }


        /// <summary>
        /// 角色id列表
        /// </summary>
        [Required]
#pragma warning disable CA2227 // 集合属性应为只读
        public List<string> RoleIds  { get; set; }
#pragma warning restore CA2227 // 集合属性应为只读
    }
}

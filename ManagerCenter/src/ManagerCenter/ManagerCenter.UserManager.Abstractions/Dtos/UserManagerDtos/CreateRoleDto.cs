using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Dtos.UserManagerDtos
{
    public class CreateRoleDto
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name  { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 一旦创建了将不可编辑
        /// </summary>
        public bool NonEditable { get; set; }
    }
}

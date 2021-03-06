﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Models.UserManagerModels
{
    public class Department
    {
        public int Id { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name  { get; set; }


        /// <summary>
        /// 部门描述
        /// </summary>

        public string Desc  { get; set; }


        /// <summary>
        /// 创建数据
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }


        /// <summary>
        /// 父部门id
        /// </summary>

        public int ParentDepartmentId  { get; set; }

        
    }
}

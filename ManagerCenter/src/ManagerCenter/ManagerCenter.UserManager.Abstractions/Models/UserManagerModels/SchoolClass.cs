// <copyright file="SchoolClass.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ManagerCenter.UserManager.Abstractions.Models.UserManagerModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class SchoolClass
    {
        public SchoolClass()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public SchoolClass(string name)
        {
            this.Id = Guid.NewGuid().ToString("N");
            this.Name = name;
        }

        /// <summary>
        /// 班级Id
        /// </summary>
        [Key]
        public string Id { get; set; }


        /// <summary>
        /// 班级名称
        /// </summary>
        [Required]
        public string Name { get; set; }


        /// <summary>
        /// 班级描述
        /// </summary>
        public string Desc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IdentityServerCenter.Models
{
    public class SchoolClass
    {
        public SchoolClass()
        {
            Id = Guid.NewGuid().ToString("N");
        }

        public SchoolClass(string name)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = name;
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

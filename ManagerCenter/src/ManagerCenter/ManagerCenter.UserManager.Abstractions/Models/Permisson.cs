using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.UserManager.Abstractions.Models
{
    /// <summary>
    /// 声明类型
    /// </summary>
    public class Permisson
    {
        [Key]
        public string Id  { get; set; }

        /// <summary>
        /// 名称（只能是数字或者字母）
        /// </summary>
        [Required]
        [RegularExpression("(?i)^[0-9a-z.]+$",ErrorMessage = "{0}只能是数字、字母或者英文.点号")]
        [DisplayName("名称")]
        public string Name  { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        [Required]
        
        public string DisplyName  { get; set; }


        /// <summary>
        /// 声明描述
        /// </summary>
        public string Desc  { get; set; }

        /// <summary>
        /// 启用？
        /// </summary>
        public bool Enabled  { get; set; }
    }
}

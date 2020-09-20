using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Models.UserManagerModels
{
    /// <summary>
    /// 字典类型
    /// </summary>
   public class DictionaryType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id  { get; set; }


        [Required]
        public string Name  { get; set; }


        public string Remark  { get; set; }


        [Timestamp]
#pragma warning disable CA1819 // 属性不应返回数组
        public byte[] RowVersion { get; set; }
#pragma warning restore CA1819 // 属性不应返回数组
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace ManagerCenter.UserManager.Abstractions.Models.UserManagerModels
{
    /// <summary>
    /// 字典
    /// </summary>
    public class Dictionary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public long DictionaryTypeId { get; set; }

        /// <summary>
        /// 代码
        /// </summary>
        [Required]
        public string Code  { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Required]
        public string Value  { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark  { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        public bool Enabled  { get; set; }

        [Timestamp]
#pragma warning disable CA1819 // 属性不应返回数组
        public byte[] RowVersion { get; set; }
#pragma warning restore CA1819 // 属性不应返回数组
    }
}

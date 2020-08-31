﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;

namespace IdentityServerCenter.Database.Models
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
        public byte[] RowVersion { get; set; }
    }
}

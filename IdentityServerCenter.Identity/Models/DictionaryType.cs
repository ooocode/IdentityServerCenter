using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace IdentityServerCenter.Database.Models
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
        public byte[] RowVersion { get; set; }
    }
}

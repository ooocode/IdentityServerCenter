using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WF.Core.Models
{
    /// <summary>
    /// 附件
    /// </summary>
    public class ArchAttachment
    {
        public int Id  { get; set; }

        /// <summary>
        /// 业务编号
        /// </summary>
        [Required]
        public string BusinessKey { get; set; }


        /// <summary>
        /// 原始文件名称
        /// </summary>
        [Required]
        public string OrignFileName  { get; set; }


        /// <summary>
        /// 当前文件名称
        /// </summary>
        [Required]
        public string  FileName { get; set; }

        [Required]
        public string ContentType  { get; set; }



        /// <summary>
        /// 创建日期（第一次上传的时间）
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }


        /// <summary>
        /// 最新的上传时间
        /// </summary>
        public DateTimeOffset LastUploadDateTime { get; set; }
    }
}

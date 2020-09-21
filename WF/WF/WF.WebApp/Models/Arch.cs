using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WF.WebApp.Models
{
    public class Arch
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string FlowStatus  { get; set; }

    
        /// <summary>
        /// 标题
        /// </summary>
        public string Title  { get; set; }

        /// <summary>
        /// 业务编号
        /// </summary>
        public string BusinessKey { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version  { get; set; }
    }
}

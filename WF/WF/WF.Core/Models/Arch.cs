using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WF.Core.Models
{
    public class Arch
    {
        public int Id { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string FlowStatus  { get; set; }

        /// <summary>
        /// 唯一业务编号
        /// </summary>
        public string BusinessKey { get; set; }

        /// <summary>
        /// 处理定义id
        /// </summary>
        public string ProcessDefinitionId { get; set; }


        /// <summary>
        /// 处理定义名称
        /// </summary>
        public string ProcessDefinitionName { get; set; }


        /// <summary>
        /// 版本号
        /// </summary>
        public int Version  { get; set; }
    }
}

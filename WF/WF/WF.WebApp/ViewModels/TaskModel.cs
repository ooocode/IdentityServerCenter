using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WF.WebApp.ViewModels
{
    public class TaskModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// 当前流程状态
        /// </summary>
        public string FlowStatus { get; set; }


        public string BusinessKey { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 目标网关条件
        /// </summary>
        public List<string> TargetGatewayConditions { get; set; }


        /// <summary>
        /// 外向
        /// </summary>
        public List<string> Outgoings { get; set; }

        public string ProcessDefinitionId { get; set; }
    }
}

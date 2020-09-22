using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WF.WebApp.ViewModels
{
    public class CompleteTaskModel
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// dispose
        /// </summary>
        public string Dispose { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }


        public string ArchNo { get; set; }

        /// <summary>
        /// 接收用户列表
        /// </summary>
        public string RecvUserList { get; set; }
    }
}

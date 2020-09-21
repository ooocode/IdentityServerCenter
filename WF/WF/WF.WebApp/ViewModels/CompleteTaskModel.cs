using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WF.WebApp.ViewModels
{
    public class CompleteTaskModel
    {
        public string TaskId { get; set; }

        public string Dispose { get; set; }

        public string Title  { get; set; }

        /// <summary>
        /// 接收用户列表
        /// </summary>
        public string RecvUserList  { get; set; }
    }
}

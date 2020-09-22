using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.Core.Models;

namespace WF.WebApp.Models
{
    public class MyArch : Arch
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title  { get; set; }


        /// <summary>
        /// 文号
        /// </summary>
        public string ArchNo  { get; set; }
    }
}

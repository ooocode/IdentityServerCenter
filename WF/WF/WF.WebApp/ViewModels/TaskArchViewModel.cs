using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.WebApp.Models;

namespace WF.WebApp.ViewModels
{
    public class TaskArchViewModel
    {
        public TaskModel Task { get; set; }

        public MyArch Arch { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WF.WebApp.ViewModels
{
    public class StartProcessDefinitionModel
    {
        public string processDefinitionId { get; set; }
        public string businessKey { get; set; }
        public Dictionary<string, object> variables { get; set; }
    }
}

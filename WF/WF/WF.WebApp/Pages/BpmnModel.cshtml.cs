using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.WebApp.Services;

namespace WF.WebApp.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly TasksService tasksService;

        public PrivacyModel(ILogger<PrivacyModel> logger,TasksService tasksService)
        {
            _logger = logger;
            this.tasksService = tasksService;
        }


        public async Task<IActionResult> OnGetLoadBpmnAsync(string processDefinitionId)
        {
            var bpmnModelXml = await tasksService.GetProcessModelXml(processDefinitionId);
            if (string.IsNullOrEmpty(bpmnModelXml))
            {
                return NotFound();
            }
            return Content(bpmnModelXml);
        }
    }
}

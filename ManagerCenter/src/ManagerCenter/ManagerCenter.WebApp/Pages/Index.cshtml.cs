using Elsa.Activities.UserTask.Activities;
using Elsa.Design;
using Elsa.Models;
using Elsa.Persistence;
using Elsa.Persistence.EntityFrameworkCore.Entities;
using Elsa.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerCenter.WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IWorkflowInvoker workflowInvoker;
        private readonly ElsaContextEx db;
        private readonly IWorkflowDefinitionStore workflowDefinitionStore;
        private readonly IWorkflowInstanceStore workflowInstanceStore;

        public IndexModel(ILogger<IndexModel> logger,IWorkflowInvoker WorkflowInvoker, ElsaContextEx db,
            IWorkflowDefinitionStore workflowDefinitionStore,
            IWorkflowInstanceStore workflowInstanceStore)
        {
            _logger = logger;
            workflowInvoker = WorkflowInvoker;
            this.workflowDefinitionStore = workflowDefinitionStore;
            this.workflowInstanceStore = workflowInstanceStore;
            this.db = db;
        }

        public async Task OnGetAsync()
        {
           
        }

        public async Task<IActionResult> OnGetStartFlowAsync(string workflowDefinitionId)
        {
            var def = await workflowDefinitionStore.GetByIdAsync(workflowDefinitionId, VersionOptions.Latest);
            if (def != null)
            {
                await workflowInvoker.StartAsync(def);
            }

            return RedirectToPage("/Index");
        }
    }
}

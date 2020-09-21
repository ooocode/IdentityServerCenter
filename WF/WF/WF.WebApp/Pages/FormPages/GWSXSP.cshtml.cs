using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WF.WebApp.Services;
using WF.WebApp.ViewModels;

namespace WF.WebApp.Pages.FormPages
{
    public class GWSXSPModel : PageModel
    {
        private readonly TasksService tasksService;

        public GWSXSPModel(TasksService tasksService)
        {
            this.tasksService = tasksService;
        }

        public ViewModels.TaskModel UndoTask { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            UndoTask = await tasksService.GetTaskByIdAsync(id);
            if (UndoTask == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync([FromForm]CompleteTaskModel completeTaskModel)
        {
            await tasksService.CompleteTaskAsync(completeTaskModel);
            return Redirect("/Index");
        }
    }
}

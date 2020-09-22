using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WF.Core.Managers;
using WF.WebApp.Models;
using WF.WebApp.Services;
using WF.WebApp.ViewModels;

namespace WF.WebApp.Pages.FormPages
{
    public class GWSXSPModel : PageModel
    {
        private readonly TasksService tasksService;
        private readonly ArchManager<MyArch> archManager;

        public GWSXSPModel(TasksService tasksService,ArchManager<MyArch> archManager)
        {
            this.tasksService = tasksService;
            this.archManager = archManager;
        }

        public ViewModels.TaskModel UndoTask { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            UndoTask = await tasksService.GetTodoTaskByIdAsync(id);
            if (UndoTask == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostCompleteAsync([FromForm]CompleteTaskModel completeTaskModel)
        {
            await tasksService.CompleteTaskAsync(completeTaskModel);
            return Redirect("/Index");
        }

    }
}

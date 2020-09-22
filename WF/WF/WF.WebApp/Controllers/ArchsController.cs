using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.Core.Managers;
using WF.WebApp.Models;
using WF.WebApp.Services;
using WF.WebApp.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WF.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchsController : ControllerBase
    {
        private readonly ArchManager<MyArch> archManager;
        private readonly TasksService taskService;

        public ArchsController(ArchManager<MyArch> archManager, TasksService taskService)
        {
            this.archManager = archManager;
            this.taskService = taskService;
        }

        /// <summary>
        /// 待办列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("todo")]
        public async IAsyncEnumerable<TaskArchViewModel> GetTodoTasksAsync()
        {
            var tasks = await taskService.GetTodoTasksAsync();
            foreach (var task in tasks)
            {
                var arch = await archManager.GetArchByBusinessKeyAsync(task.BusinessKey);
                yield return new TaskArchViewModel
                {
                    Task = task,
                    Arch = arch
                };
            }
        }


        /// <summary>
        /// 通过id获取待办
        /// </summary>
        /// <returns></returns>
        [HttpGet("todo/{taskId}")]
        public async Task<TaskArchViewModel> GetTodoTaskByIdAsync(string taskId)
        {
            var task = await taskService.GetTodoTaskByIdAsync(taskId);
            if (task == null)
            {
                return null;
            }

            var arch = await archManager.GetArchByBusinessKeyAsync(task.BusinessKey);
            return new TaskArchViewModel
            {
                Task = task,
                Arch = arch
            };
        }



        /// <summary>
        /// 待办列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("done")]
        public async IAsyncEnumerable<TaskArchViewModel> GetDoneTasksAsync()
        {
            var tasks = await taskService.GetDoneTasksAsync();
            foreach (var task in tasks)
            {
                var arch = await archManager.GetArchByBusinessKeyAsync(task.BusinessKey);
                yield return new TaskArchViewModel
                {
                    Task = task,
                    Arch = arch
                };
            }
        }


        /// <summary>
        /// 通过id获取待办
        /// </summary>
        /// <returns></returns>
        [HttpGet("done/{taskId}")]
        public async Task<TaskArchViewModel> GetDoneTaskByIdAsync(string taskId)
        {
            var task = await taskService.GetTodoTaskByIdAsync(taskId);
            if (task == null)
            {
                return null;
            }

            var arch = await archManager.GetArchByBusinessKeyAsync(task.BusinessKey);
            return new TaskArchViewModel
            {
                Task = task,
                Arch = arch
            };
        }



        /// <summary>
        /// 上传附件
        /// </summary>
        /// <param name="businessKey"></param>
        /// <returns></returns>
        [HttpPost("{businessKey}/attachments")]
        public async Task<IActionResult> AddArchAttachmentAsync(string businessKey)
        {
            IFormFile formFile = HttpContext.Request.Form.Files.FirstOrDefault();
            if (formFile == null)
            {
                return NotFound();
            }

            var arch = await archManager.GetArchByBusinessKeyAsync(businessKey);
            if (arch == null)
            {
                return NotFound();
            }

            await archManager.AddArchAttachmentAsync(arch, formFile);
            return Ok();
        }


        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="businessKey"></param>
        /// <returns></returns>
        // DELETE api/<ArchsController>/5
        [HttpDelete("{businessKey}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteArchAttachmentAsync(string businessKey, int attachmentId)
        {
            var arch = await archManager.GetArchByBusinessKeyAsync(businessKey);
            if (arch == null)
            {
                return NotFound();
            }
            await archManager.DeleteArchAttachments(arch, new List<int> { attachmentId });
            return Ok();
        }
    }
}

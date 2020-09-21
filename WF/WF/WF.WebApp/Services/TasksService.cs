using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WF.WebApp.Data;
using WF.WebApp.ViewModels;

namespace WF.WebApp.Services
{
    public class TasksService
    {
        private readonly HttpClient httpClient;
        private readonly ArchDbContext archDbContext;

        public TasksService(HttpClient httpClient, ArchDbContext archDbContext)
        {
            this.httpClient = httpClient;
            this.archDbContext = archDbContext;
        }

        /// <summary>
        /// 启动处理流程
        /// </summary>
        /// <param name="processDefinitionId"></param>
        /// <returns></returns>
        public async Task<string> startProcessDefinitionByIdAsync(string processDefinitionId)
        {
            StartProcessDefinitionModel model = new StartProcessDefinitionModel
            {
                businessKey = Guid.NewGuid().ToString(),
                processDefinitionId = processDefinitionId,
                variables = new Dictionary<string, object>()
            };

            model.variables.Add("assignee", "lhl");

            var arch = await archDbContext.Arches.FirstOrDefaultAsync(e => e.BusinessKey == model.businessKey);
            if (arch == null)
            {
                arch = new Models.Arch
                {
                    BusinessKey = model.businessKey,
                    Version = 0
                };

                await archDbContext.Arches.AddAsync(arch);
                await archDbContext.SaveChangesAsync();
            }

            var result = await httpClient.PostAsJsonAsync($"/startProcessDefinitionById?processDefinitionId", model);
            var text = await result.RequestMessage.Content.ReadAsStringAsync();
            return text;
        }

        /// <summary>
        /// 通过id获取任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TaskModel> GetTaskByIdAsync(string id)
        {
            var result = await httpClient.GetAsync($"/getTaskById?id={id}");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                var task = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskModel>(str);
                return task;
            }
            return null;
        }


        /// <summary>
        /// 获取处理定义xml
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetProcessModelXml(string processDefinitionId)
        {
            var result = await httpClient.GetAsync($"/getProcessModelXml?processDefinitionId={processDefinitionId}");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                return str;
            }

            return null;
        }

        /// <summary>
        /// 获取未办任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TaskModel>> GetTasks()
        {
            var result = await httpClient.GetAsync($"/getTasks");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                var ls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskModel>>(str);
                return ls;
            }
            return null;
        }

        /// <summary>
        /// 获取已办办任务
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<TaskModel>> DoneTasks()
        {
            var result = await httpClient.GetAsync($"/doneTasks");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                var ls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TaskModel>>(str);
                return ls;
            }
            return null;
        }



        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="model"></param>
        public async Task<string> CompleteTaskAsync(CompleteTaskModel model)
        {
            var task = await GetTaskByIdAsync(model.TaskId);
            if (task == null)
            {
                return null;
            }


            var arch = await archDbContext.Arches.FirstOrDefaultAsync(e => e.BusinessKey == task.BusinessKey);
            if (arch != null)
            {
                arch.FlowStatus = task.FlowStatus;
                arch.Title = model.Title;
                arch.Version = arch.Version + 1;

                await archDbContext.SaveChangesAsync();
            }

            Dictionary<string, object> Variables = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(model.Dispose))
            {
                Variables.Add("dispose", model.Dispose);
            }

            if (!string.IsNullOrEmpty(model.RecvUserList))
            {
                Variables.Add("assignee", model.RecvUserList);
            }


            var sendModel = new
            {
                TaskId = model.TaskId,
                Variables
            };

            var result = await httpClient.PostAsJsonAsync($"/completeTask", sendModel);
            var text = await result.RequestMessage.Content.ReadAsStringAsync();
            return text;
        }


    }
}

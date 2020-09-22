using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WF.Core.Data;
using WF.Core.Models;
using WF.WebApp.Models;
using WF.WebApp.ViewModels;

namespace WF.WebApp.Services
{
    public class TasksService
    {
        private readonly HttpClient httpClient;
        private readonly ArchDbContext<MyArch> archDbContext;

        public TasksService(HttpClient httpClient, ArchDbContext<MyArch> archDbContext)
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
            var processDef = await GetProcessDefinitionById(processDefinitionId);
            if(processDef == null)
            {
                return null;
            }

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
                arch = new MyArch
                {
                    BusinessKey = model.businessKey,
                    ProcessDefinitionId = processDefinitionId,
                    ProcessDefinitionName = processDef.Name,
                    ArchNo = $"{processDef.Name} {DateTime.Now.Year} 号",
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
        /// 获取处理定义列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<ProcessDefinitionModel>> GetProcessDefinitions()
        {
            var result = await httpClient.GetAsync($"/getProcessDefinitions");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                var processDefinitions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProcessDefinitionModel>>(str);
                return processDefinitions;
            }
            return null;
        }




        /// <summary>
        /// 通过id获取处理定义
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProcessDefinitionModel> GetProcessDefinitionById(string processDefinitionId)
        {
            var result = await httpClient.GetAsync($"/getProcessDefinitionById?processDefinitionId={processDefinitionId}");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                var processDefinition = Newtonsoft.Json.JsonConvert.DeserializeObject<ProcessDefinitionModel>(str);
                return processDefinition;
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
        public async Task<List<TaskModel>> GetTodoTasksAsync()
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
        public async Task<List<TaskModel>> GetDoneTasksAsync()
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


        public async Task<TaskModel> GetTodoTaskByIdAsync(string id)
        {
            var result = await httpClient.GetAsync($"/getTaskById?id={id}");
            if (result.IsSuccessStatusCode)
            {
                var str = await result.Content.ReadAsStringAsync();
                var ls = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskModel>(str);
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
            var task = await GetTodoTaskByIdAsync(model.TaskId);
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
                arch.ArchNo = model.ArchNo;

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

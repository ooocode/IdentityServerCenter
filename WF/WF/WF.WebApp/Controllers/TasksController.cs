using Camunda.Api.Client;
using Camunda.Api.Client.ProcessDefinition;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WF.WebApp.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WF.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly CamundaClient camunda;
        private readonly TasksService tasksService;

        public TasksController(CamundaClient camunda, TasksService tasksService)
        {
            this.camunda = camunda;
            this.tasksService = tasksService;
        }

        [HttpGet("StartTask")]
        public async Task<IActionResult> StartTaskAsync(string processDefinitionId)
        {
            var result = await tasksService.startProcessDefinitionByIdAsync(processDefinitionId);
            return Redirect("/Index");
        }


        // GET: api/<TasksController>
        [HttpGet]
        public IEnumerable<string> UndoTasks()
        {
          
            return new string[] { "value1", "value2" };
        }

        // GET api/<TasksController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<TasksController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TasksController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TasksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

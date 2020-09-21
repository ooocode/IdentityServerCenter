package com.study.workflow;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.study.workflow.Models.*;

import org.camunda.bpm.engine.HistoryService;
import org.camunda.bpm.engine.RepositoryService;
import org.camunda.bpm.engine.RuntimeService;
import org.camunda.bpm.engine.TaskService;
import org.camunda.bpm.engine.history.HistoricActivityInstance;
import org.camunda.bpm.engine.history.HistoricProcessInstance;
import org.camunda.bpm.engine.repository.ProcessDefinition;
import org.camunda.bpm.engine.runtime.ProcessInstance;
import org.camunda.bpm.engine.task.Task;
import org.camunda.bpm.model.bpmn.Bpmn;
import org.camunda.bpm.model.bpmn.BpmnModelInstance;
import org.camunda.bpm.model.bpmn.impl.instance.ExclusiveGatewayImpl;
import org.camunda.bpm.model.bpmn.impl.instance.UserTaskImpl;
import org.camunda.bpm.model.bpmn.instance.Activity;
import org.camunda.bpm.model.bpmn.instance.EndEvent;
import org.camunda.bpm.model.bpmn.instance.ExclusiveGateway;
import org.h2.util.StringUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.ResponseBody;
import org.springframework.web.bind.annotation.RestController;

@RestController
@CrossOrigin
public class TasksController {
    @Autowired
    private TaskService taskService;

    @Autowired
    private RepositoryService repositoryService;

    @Autowired
    private RuntimeService runtimeService;

    @Autowired
    private HistoryService historyService;


    /**
     * 获取处理定义列表
     *
     * @return
     */
    @GetMapping("/getProcessDefinitions")
    @ResponseBody
    public List<ProcessDefinitionModel> getProcessDefinitions() {
        List<ProcessDefinitionModel> models = new ArrayList<>();

        final List<ProcessDefinition> ls = repositoryService.createProcessDefinitionQuery()
                .orderByProcessDefinitionVersion().desc().list();
        for (ProcessDefinition item : ls) {
            ProcessDefinitionModel model = ProcessDefinitionModel.fromEntity(item);
            models.add(model);
        }

        return models;
    }

    /**
     * 通过id获取处理定义
     *
     * @param processDefinitionId
     * @return
     */
    @GetMapping("/getProcessDefinitionById")
    @ResponseBody
    public ProcessDefinitionModel getProcessDefinitionById(String processDefinitionId) {
        final ProcessDefinition p = repositoryService.createProcessDefinitionQuery()
                .processDefinitionId(processDefinitionId).singleResult();

        ProcessDefinitionModel model = ProcessDefinitionModel.fromEntity(p);

        return model;
    }

    /**
     * 通过id启动处理定义
     *
     * @param processDefinitionId 处理定义id
     * @return 处理实例id
     */
    @PostMapping("/startProcessDefinitionById")
    @ResponseBody
    public String startProcessDefinitionById(@RequestBody StartProcessDefinitionModel model) {
        ProcessInstance instance = runtimeService.startProcessInstanceById(model.getProcessDefinitionId(),
                model.getBusinessKey(), model.getVariables());
        return instance.getId();
    }

    /**
     * 获取未完成的任务
     *
     * @return 任务列表
     */
    @GetMapping("/getTasks")
    @ResponseBody
    public List<TaskModel> getTasks() {
        List<TaskModel> models = new ArrayList<>();
        var ls = taskService.createTaskQuery().orderByTaskCreateTime().desc().list();
        for (Task task : ls) {
            var processInstance = runtimeService.createProcessInstanceQuery()
                    .processInstanceId(task.getProcessInstanceId()).singleResult();
            if (processInstance == null) {
                continue;
            }

            var taskModel = TaskModel.fromTaskEntity(task);
            taskModel.setBusinessKey(processInstance.getBusinessKey());
            models.add(taskModel);
        }
        return models;
    }

    /**
     * 通过id获取未完成的任务
     *
     * @param id 任务id
     * @return 任务
     */
    @GetMapping("/getTaskById")
    @ResponseBody
    public TaskModel getTaskById(String id) {
        Task task = taskService.createTaskQuery().taskId(id).singleResult();
        if (task == null) {
            return null;
        }

        var processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(task.getProcessInstanceId())
                .singleResult();
        if (processInstance == null) {
            return null;
        }

        TaskModel taskModel = TaskModel.fromTaskEntity(task);
        taskModel.setTargetGatewayConditions(new ArrayList<>());
        taskModel.setOutgoings(new ArrayList<>());
        taskModel.setBusinessKey(processInstance.getBusinessKey());

        // 当前任务所在的节点id
        var taskDefinitionKey = task.getTaskDefinitionKey();
        if (!StringUtils.isNullOrEmpty(taskDefinitionKey)) {
            var stream = repositoryService.getProcessModel(task.getProcessDefinitionId());
            BpmnModelInstance modelInstance = Bpmn.readModelFromStream(stream);
            var curNode = modelInstance.getModelElementById(taskDefinitionKey);
            if (curNode.getClass().equals(UserTaskImpl.class)) {
                Activity activity = (Activity) curNode;

                var outgoings = activity.getOutgoing();
                outgoings.forEach(outgoing -> {
                    var name = StringUtils.isNullOrEmpty(outgoing.getName()) ? outgoing.getId() : outgoing.getName();
                    taskModel.getOutgoings().add(name);

                    var targetFlowNode = outgoing.getTarget();
                    if (targetFlowNode.getClass().equals(ExclusiveGatewayImpl.class)) {
                        var exclusiveGateway = (ExclusiveGateway) targetFlowNode;
                        exclusiveGateway.getOutgoing().forEach(o -> {
                            var condition = o.getConditionExpression();
                            if (condition != null) {
                                var conditionTextContent = condition.getTextContent();
                                taskModel.getTargetGatewayConditions().add(conditionTextContent);
                            }
                        });
                    }
                });
            }
        }

        return taskModel;
    }

    /**
     * 完成任务
     *
     * @param model
     */
    @PostMapping("/completeTask")
    @ResponseBody
    public Result completeTask(@RequestBody CompleteTaskModel model) {
        Result result = new Result();
        try {
            taskService.complete(model.getTaskId(), model.getVariables());
            result.setSuccessed(true);
        } catch (Exception ex) {
            result.setErrorMessage(ex.getMessage());
        }
        return  result;
    }


    /**
     * 已办完成任务
     */
    @GetMapping("/doneTasks")
    @ResponseBody
    public List<TaskModel> doneTasks() {
        List<TaskModel> taskModels = new ArrayList<>();
        try {
            var taskInstances = historyService.createHistoricTaskInstanceQuery()
                    .finished()
                    .orderByHistoricTaskInstanceEndTime()
                    .desc()
                    .list();
            taskInstances.forEach(e -> {
                //获取处理实例Id
                final String proccessIntanceId = e.getProcessInstanceId();

                //通过处理实例Id获取历史处理实例
                var processInstance = historyService.createHistoricProcessInstanceQuery().processInstanceId(proccessIntanceId).singleResult();

                var businessKey = processInstance.getBusinessKey();
                TaskModel taskModel = new TaskModel();
                taskModel.setBusinessKey(businessKey);
                taskModel.setUserName(e.getAssignee());
                taskModel.setFlowStatus(e.getName());
                taskModel.setCreateTime(e.getEndTime());
                taskModel.setProcessDefinitionId(e.getProcessDefinitionId());


                //var task = taskService.createTaskQuery().taskId(id).singleResult();


                //var taskModel = TaskModel.fromTaskEntity(task);
                //taskModel.setBusinessKey(processInstance.getBusinessKey());

                taskModels.add(taskModel);
            });

        } catch (Exception ex) {

        }
        return taskModels;
    }

    @GetMapping("/getProcessModelXml")
    public String getProcessModelXml(String processDefinitionId) {

        InputStream stream = repositoryService.getProcessModel(processDefinitionId);
        byte[] bytes = new byte[0];
        try {
            bytes = stream.readAllBytes();
        } catch (IOException e) {
            e.printStackTrace();
        }
        String xml = new String(bytes);

        return xml;
    }
}
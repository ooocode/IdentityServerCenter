package com.study.workflow.Models;

import java.util.Date;
import java.util.List;

import org.camunda.bpm.engine.task.Task;

public class TaskModel {
    private String id;

    /**
     * 当前流程状态
     */
    private String flowStatus;

    /**
     * 业务编号
     */
    private String businessKey;

    /**
     * 创建时间
     */

    private Date createTime;


    /**
     * 任务的用户名
     */
    private String userName;

    /**
     * 目标条件
     */
    private List<String> targetGatewayConditions;

    private List<String> outgoings;

    public static TaskModel fromTaskEntity(Task task) {
        TaskModel model = new TaskModel();
        model.setId(task.getId());
        model.setFlowStatus(task.getName());
        model.setCreateTime(task.getCreateTime());
        model.setUserName(task.getAssignee());
        return model;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getFlowStatus() {
        return flowStatus;
    }

    public void setFlowStatus(String flowStatus) {
        this.flowStatus = flowStatus;
    }

    public List<String> getTargetGatewayConditions() {
        return targetGatewayConditions;
    }

    public void setTargetGatewayConditions(List<String> targetGatewayConditions) {
        this.targetGatewayConditions = targetGatewayConditions;

    }

    public List<String> getOutgoings() {
        return outgoings;
    }

    public void setOutgoings(List<String> outgoings) {
        this.outgoings = outgoings;
    }

    public String getBusinessKey() {
        return businessKey;
    }

    public void setBusinessKey(String businessKey) {
        this.businessKey = businessKey;
    }

    public Date getCreateTime() {
        return createTime;
    }

    public void setCreateTime(Date createTime) {
        this.createTime = createTime;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }
}

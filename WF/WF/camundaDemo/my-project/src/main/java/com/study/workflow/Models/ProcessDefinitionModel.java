package com.study.workflow.Models;

import org.camunda.bpm.engine.repository.ProcessDefinition;

public class ProcessDefinitionModel {
    private String name;
    private String id;
    private String processDefinitionKey;

    public static ProcessDefinitionModel fromEntity(ProcessDefinition p) {
        ProcessDefinitionModel model = new ProcessDefinitionModel();
        model.setId(p.getId());
        model.setName(p.getName());
        model.setProcessDefinitionKey(p.getKey());
        return model;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getProcessDefinitionKey() {
        return processDefinitionKey;
    }

    public void setProcessDefinitionKey(String processDefinitionKey) {
        this.processDefinitionKey = processDefinitionKey;
    }

}

package com.study.workflow.Models;

public class Result {
    private boolean isSuccessed;
    private String errorMessage;

    public boolean isSuccessed() {
        return isSuccessed;
    }

    public void setSuccessed(boolean isSuccessed) {
        this.isSuccessed = isSuccessed;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }
}

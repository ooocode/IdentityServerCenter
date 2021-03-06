/* tslint:disable */
/* eslint-disable */
//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v13.7.0.0 (NJsonSchema v10.1.24.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------
// ReSharper disable InconsistentNaming

import axios, { AxiosError, AxiosInstance, AxiosRequestConfig, AxiosResponse, CancelToken } from 'axios';

export class ArchsClient {
    private instance: AxiosInstance;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, instance?: AxiosInstance) {
        this.instance = instance ? instance : axios.create();
        this.baseUrl = baseUrl ? baseUrl : "http://localhost:5000";
    }

    /**
     * 待办列表
     */
    getTodoTasks(  cancelToken?: CancelToken | undefined): Promise<TaskArchViewModel[]> {
        let url_ = this.baseUrl + "/api/Archs/todo";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/json"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processGetTodoTasks(_response);
        });
    }

    protected processGetTodoTasks(response: AxiosResponse): Promise<TaskArchViewModel[]> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(TaskArchViewModel.fromJS(item));
            }
            return result200;
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<TaskArchViewModel[]>(<any>null);
    }

    /**
     * 通过id获取待办
     */
    getTodoTaskById(taskId: string | null , cancelToken?: CancelToken | undefined): Promise<TaskArchViewModel> {
        let url_ = this.baseUrl + "/api/Archs/todo/{taskId}";
        if (taskId === undefined || taskId === null)
            throw new Error("The parameter 'taskId' must be defined.");
        url_ = url_.replace("{taskId}", encodeURIComponent("" + taskId));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/json"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processGetTodoTaskById(_response);
        });
    }

    protected processGetTodoTaskById(response: AxiosResponse): Promise<TaskArchViewModel> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
            result200 = TaskArchViewModel.fromJS(resultData200);
            return result200;
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<TaskArchViewModel>(<any>null);
    }

    /**
     * 待办列表
     */
    getDoneTasks(  cancelToken?: CancelToken | undefined): Promise<TaskArchViewModel[]> {
        let url_ = this.baseUrl + "/api/Archs/done";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/json"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processGetDoneTasks(_response);
        });
    }

    protected processGetDoneTasks(response: AxiosResponse): Promise<TaskArchViewModel[]> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(TaskArchViewModel.fromJS(item));
            }
            return result200;
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<TaskArchViewModel[]>(<any>null);
    }

    /**
     * 通过id获取待办
     */
    getDoneTaskById(taskId: string | null , cancelToken?: CancelToken | undefined): Promise<TaskArchViewModel> {
        let url_ = this.baseUrl + "/api/Archs/done/{taskId}";
        if (taskId === undefined || taskId === null)
            throw new Error("The parameter 'taskId' must be defined.");
        url_ = url_.replace("{taskId}", encodeURIComponent("" + taskId));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/json"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processGetDoneTaskById(_response);
        });
    }

    protected processGetDoneTaskById(response: AxiosResponse): Promise<TaskArchViewModel> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
            result200 = TaskArchViewModel.fromJS(resultData200);
            return result200;
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<TaskArchViewModel>(<any>null);
    }

    /**
     * 上传附件
     */
    addArchAttachment(businessKey: string | null , cancelToken?: CancelToken | undefined): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Archs/{businessKey}/attachments";
        if (businessKey === undefined || businessKey === null)
            throw new Error("The parameter 'businessKey' must be defined.");
        url_ = url_.replace("{businessKey}", encodeURIComponent("" + businessKey));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            responseType: "blob",
            method: "POST",
            url: url_,
            headers: {
                "Accept": "application/octet-stream"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processAddArchAttachment(_response);
        });
    }

    protected processAddArchAttachment(response: AxiosResponse): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers["content-disposition"] : undefined;
            const fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
            const fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            return Promise.resolve({ fileName: fileName, status: status, data: response.data as Blob, headers: _headers });
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<FileResponse>(<any>null);
    }

    /**
     * 删除附件
     */
    deleteArchAttachment(businessKey: string | null, attachmentId: number , cancelToken?: CancelToken | undefined): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Archs/{businessKey}/attachments/{attachmentId}";
        if (businessKey === undefined || businessKey === null)
            throw new Error("The parameter 'businessKey' must be defined.");
        url_ = url_.replace("{businessKey}", encodeURIComponent("" + businessKey));
        if (attachmentId === undefined || attachmentId === null)
            throw new Error("The parameter 'attachmentId' must be defined.");
        url_ = url_.replace("{attachmentId}", encodeURIComponent("" + attachmentId));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            responseType: "blob",
            method: "DELETE",
            url: url_,
            headers: {
                "Accept": "application/octet-stream"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processDeleteArchAttachment(_response);
        });
    }

    protected processDeleteArchAttachment(response: AxiosResponse): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers["content-disposition"] : undefined;
            const fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
            const fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            return Promise.resolve({ fileName: fileName, status: status, data: response.data as Blob, headers: _headers });
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<FileResponse>(<any>null);
    }
}

export class TasksClient {
    private instance: AxiosInstance;
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, instance?: AxiosInstance) {
        this.instance = instance ? instance : axios.create();
        this.baseUrl = baseUrl ? baseUrl : "http://localhost:5000";
    }

    startTask(processDefinitionId: string | null | undefined , cancelToken?: CancelToken | undefined): Promise<FileResponse> {
        let url_ = this.baseUrl + "/api/Tasks/StartTask?";
        if (processDefinitionId !== undefined && processDefinitionId !== null)
            url_ += "processDefinitionId=" + encodeURIComponent("" + processDefinitionId) + "&";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            responseType: "blob",
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/octet-stream"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processStartTask(_response);
        });
    }

    protected processStartTask(response: AxiosResponse): Promise<FileResponse> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200 || status === 206) {
            const contentDisposition = response.headers ? response.headers["content-disposition"] : undefined;
            const fileNameMatch = contentDisposition ? /filename="?([^"]*?)"?(;|$)/g.exec(contentDisposition) : undefined;
            const fileName = fileNameMatch && fileNameMatch.length > 1 ? fileNameMatch[1] : undefined;
            return Promise.resolve({ fileName: fileName, status: status, data: response.data as Blob, headers: _headers });
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<FileResponse>(<any>null);
    }

    undoTasks(  cancelToken?: CancelToken | undefined): Promise<string[]> {
        let url_ = this.baseUrl + "/api/Tasks";
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/json"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processUndoTasks(_response);
        });
    }

    protected processUndoTasks(response: AxiosResponse): Promise<string[]> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
            if (Array.isArray(resultData200)) {
                result200 = [] as any;
                for (let item of resultData200)
                    result200!.push(item);
            }
            return result200;
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<string[]>(<any>null);
    }

    post(value: string , cancelToken?: CancelToken | undefined): Promise<void> {
        let url_ = this.baseUrl + "/api/Tasks";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(value);

        let options_ = <AxiosRequestConfig>{
            data: content_,
            method: "POST",
            url: url_,
            headers: {
                "Content-Type": "application/json",
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processPost(_response);
        });
    }

    protected processPost(response: AxiosResponse): Promise<void> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            return Promise.resolve<void>(<any>null);
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<void>(<any>null);
    }

    get(id: number , cancelToken?: CancelToken | undefined): Promise<string> {
        let url_ = this.baseUrl + "/api/Tasks/{id}";
        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
        url_ = url_.replace("{id}", encodeURIComponent("" + id));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "GET",
            url: url_,
            headers: {
                "Accept": "application/json"
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processGet(_response);
        });
    }

    protected processGet(response: AxiosResponse): Promise<string> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            let result200: any = null;
            let resultData200  = _responseText;
            result200 = resultData200 !== undefined ? resultData200 : <any>null;
            return result200;
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<string>(<any>null);
    }

    put(id: number, value: string , cancelToken?: CancelToken | undefined): Promise<void> {
        let url_ = this.baseUrl + "/api/Tasks/{id}";
        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
        url_ = url_.replace("{id}", encodeURIComponent("" + id));
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(value);

        let options_ = <AxiosRequestConfig>{
            data: content_,
            method: "PUT",
            url: url_,
            headers: {
                "Content-Type": "application/json",
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processPut(_response);
        });
    }

    protected processPut(response: AxiosResponse): Promise<void> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            return Promise.resolve<void>(<any>null);
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<void>(<any>null);
    }

    delete(id: number , cancelToken?: CancelToken | undefined): Promise<void> {
        let url_ = this.baseUrl + "/api/Tasks/{id}";
        if (id === undefined || id === null)
            throw new Error("The parameter 'id' must be defined.");
        url_ = url_.replace("{id}", encodeURIComponent("" + id));
        url_ = url_.replace(/[?&]$/, "");

        let options_ = <AxiosRequestConfig>{
            method: "DELETE",
            url: url_,
            headers: {
            },
            cancelToken
        };

        return this.instance.request(options_).catch((_error: any) => {
            if (isAxiosError(_error) && _error.response) {
                return _error.response;
            } else {
                throw _error;
            }
        }).then((_response: AxiosResponse) => {
            return this.processDelete(_response);
        });
    }

    protected processDelete(response: AxiosResponse): Promise<void> {
        const status = response.status;
        let _headers: any = {};
        if (response.headers && typeof response.headers === "object") {
            for (let k in response.headers) {
                if (response.headers.hasOwnProperty(k)) {
                    _headers[k] = response.headers[k];
                }
            }
        }
        if (status === 200) {
            const _responseText = response.data;
            return Promise.resolve<void>(<any>null);
        } else if (status !== 200 && status !== 204) {
            const _responseText = response.data;
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
        }
        return Promise.resolve<void>(<any>null);
    }
}

export class TaskArchViewModel implements ITaskArchViewModel {
    task?: TaskModel | undefined;
    arch?: MyArch | undefined;

    constructor(data?: ITaskArchViewModel) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.task = _data["task"] ? TaskModel.fromJS(_data["task"]) : <any>undefined;
            this.arch = _data["arch"] ? MyArch.fromJS(_data["arch"]) : <any>undefined;
        }
    }

    static fromJS(data: any): TaskArchViewModel {
        data = typeof data === 'object' ? data : {};
        let result = new TaskArchViewModel();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["task"] = this.task ? this.task.toJSON() : <any>undefined;
        data["arch"] = this.arch ? this.arch.toJSON() : <any>undefined;
        return data; 
    }
}

export interface ITaskArchViewModel {
    task?: TaskModel | undefined;
    arch?: MyArch | undefined;
}

export class TaskModel implements ITaskModel {
    id?: string | undefined;
    userName?: string | undefined;
    /** 当前流程状态 */
    flowStatus?: string | undefined;
    businessKey?: string | undefined;
    /** 创建时间 */
    createTime?: Date;
    /** 目标网关条件 */
    targetGatewayConditions?: string[] | undefined;
    /** 外向 */
    outgoings?: string[] | undefined;
    processDefinitionId?: string | undefined;

    constructor(data?: ITaskModel) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.id = _data["id"];
            this.userName = _data["userName"];
            this.flowStatus = _data["flowStatus"];
            this.businessKey = _data["businessKey"];
            this.createTime = _data["createTime"] ? new Date(_data["createTime"].toString()) : <any>undefined;
            if (Array.isArray(_data["targetGatewayConditions"])) {
                this.targetGatewayConditions = [] as any;
                for (let item of _data["targetGatewayConditions"])
                    this.targetGatewayConditions!.push(item);
            }
            if (Array.isArray(_data["outgoings"])) {
                this.outgoings = [] as any;
                for (let item of _data["outgoings"])
                    this.outgoings!.push(item);
            }
            this.processDefinitionId = _data["processDefinitionId"];
        }
    }

    static fromJS(data: any): TaskModel {
        data = typeof data === 'object' ? data : {};
        let result = new TaskModel();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["userName"] = this.userName;
        data["flowStatus"] = this.flowStatus;
        data["businessKey"] = this.businessKey;
        data["createTime"] = this.createTime ? this.createTime.toISOString() : <any>undefined;
        if (Array.isArray(this.targetGatewayConditions)) {
            data["targetGatewayConditions"] = [];
            for (let item of this.targetGatewayConditions)
                data["targetGatewayConditions"].push(item);
        }
        if (Array.isArray(this.outgoings)) {
            data["outgoings"] = [];
            for (let item of this.outgoings)
                data["outgoings"].push(item);
        }
        data["processDefinitionId"] = this.processDefinitionId;
        return data; 
    }
}

export interface ITaskModel {
    id?: string | undefined;
    userName?: string | undefined;
    /** 当前流程状态 */
    flowStatus?: string | undefined;
    businessKey?: string | undefined;
    /** 创建时间 */
    createTime?: Date;
    /** 目标网关条件 */
    targetGatewayConditions?: string[] | undefined;
    /** 外向 */
    outgoings?: string[] | undefined;
    processDefinitionId?: string | undefined;
}

export class Arch implements IArch {
    id?: number;
    flowStatus?: string | undefined;
    businessKey?: string | undefined;
    processDefinitionId?: string | undefined;
    processDefinitionName?: string | undefined;
    version?: number;

    constructor(data?: IArch) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.id = _data["id"];
            this.flowStatus = _data["flowStatus"];
            this.businessKey = _data["businessKey"];
            this.processDefinitionId = _data["processDefinitionId"];
            this.processDefinitionName = _data["processDefinitionName"];
            this.version = _data["version"];
        }
    }

    static fromJS(data: any): Arch {
        data = typeof data === 'object' ? data : {};
        let result = new Arch();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["id"] = this.id;
        data["flowStatus"] = this.flowStatus;
        data["businessKey"] = this.businessKey;
        data["processDefinitionId"] = this.processDefinitionId;
        data["processDefinitionName"] = this.processDefinitionName;
        data["version"] = this.version;
        return data; 
    }
}

export interface IArch {
    id?: number;
    flowStatus?: string | undefined;
    businessKey?: string | undefined;
    processDefinitionId?: string | undefined;
    processDefinitionName?: string | undefined;
    version?: number;
}

export class MyArch extends Arch implements IMyArch {
    /** 标题 */
    title?: string | undefined;
    /** 文号 */
    archNo?: string | undefined;

    constructor(data?: IMyArch) {
        super(data);
    }

    init(_data?: any) {
        super.init(_data);
        if (_data) {
            this.title = _data["title"];
            this.archNo = _data["archNo"];
        }
    }

    static fromJS(data: any): MyArch {
        data = typeof data === 'object' ? data : {};
        let result = new MyArch();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["title"] = this.title;
        data["archNo"] = this.archNo;
        super.toJSON(data);
        return data; 
    }
}

export interface IMyArch extends IArch {
    /** 标题 */
    title?: string | undefined;
    /** 文号 */
    archNo?: string | undefined;
}

export interface FileResponse {
    data: Blob;
    status: number;
    fileName?: string;
    headers?: { [name: string]: any };
}

export class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isApiException = true;

    static isApiException(obj: any): obj is ApiException {
        return obj.isApiException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new ApiException(message, status, response, headers, null);
}

function isAxiosError(obj: any | undefined): obj is AxiosError {
    return obj && obj.isAxiosError === true;
}
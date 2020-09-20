import Axios from "axios";

export interface IProcessDefinitionModel {
    name: string | undefined;
    id: string | undefined;
    processDefinitionKey: string | undefined;
}
/*tsModel1*/
export class ProcessDefinitionModel implements IProcessDefinitionModel {
    name: string | undefined;
    id: string | undefined;
    processDefinitionKey: string | undefined;

    /**
     *
     */
    constructor() {

    }
}

const baseUrl: string = "http://127.0.0.1:8080"

export class TasksService {
    /**
     *
     */
    constructor() {

    }

    /**
     * 获取处理定义列表
     */
    async getProcessDefinitions(): Promise<ProcessDefinitionModel[]> {
        var result = await Axios.get(baseUrl + "/getProcessDefinitions")
        var ls = result.data as ProcessDefinitionModel[]
        return ls;
    }

    /**
     * 通过id获取处理定义
     * @param processDefinitionId 处理定义id
     */
    async getProcessDefinitionById(processDefinitionId: string): Promise<ProcessDefinitionModel> {
        var result = await Axios.get(baseUrl + "/getProcessDefinitionById?processDefinitionId=" + processDefinitionId)
        return result.data as ProcessDefinitionModel
    }

    /**
     * 启动处理定义
     * @param processDefinitionId 处理定义id
     */
    async startProcessDefinitionById(processDefinitionId: string): Promise<string> {
        var result = await Axios.get(baseUrl + "/startProcessDefinitionById?processDefinitionId=" + processDefinitionId)
        return result.data as string
    }
}
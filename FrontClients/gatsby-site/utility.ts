import { notification } from "antd";
import { ApiException } from "./api";

export function showErrorMsgBox(ex: string | ApiException) {
    if (ex instanceof ApiException) {
        notification["error"]({
            message: '网络请求发生错误',
            description: (ex as ApiException).message
        });
    } else {
        notification["error"]({
            message: '发生错误',
            description: ex
        });
    }
}

export function messageBox(text: string | ApiException, type: "success" | "error") {
    if (type === "error") {
        if (text instanceof ApiException) {
            notification["error"]({
                message: '网络请求发生错误',
                description: (text as ApiException).message
            });
        } else {
            notification["error"]({
                message: '发生错误',
                description: text
            });
        }
    } else if (type === "success") {
        notification["success"]({
            message: '操作成功',
            description: text
        });
    }

}
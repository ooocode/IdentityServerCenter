import { ApiException } from "./api";

export function showErrorMsgBox(ex: string | ApiException) {
    if (ex instanceof ApiException) {
      
    } else {
     
    }
}

export function messageBox(text: string | ApiException, type: "success" | "error") {
    if (type === "error") {
        if (text instanceof ApiException) {
          
        } else {
          
        }
    } else if (type === "success") {
       
    }

}
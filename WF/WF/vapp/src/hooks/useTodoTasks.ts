import { TaskArchViewModel, ArchsClient } from './../ArchApi';

import { ref } from 'vue';

export default () => {
    let isLoading = ref(true)
    let todoTasks = ref<TaskArchViewModel[]>()
    let archsClient = new ArchsClient()

    let load = async () => {
        isLoading.value = true

        try {
            let res = await archsClient.getTodoTasks()
            todoTasks.value = res
        } catch (ex) {
            console.log(ex)
        }
        isLoading.value = false
    }

    return {
        isLoading, todoTasks, load
    }
}
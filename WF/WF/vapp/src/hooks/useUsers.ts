import { ApplicationUser, UsersClient } from './../api';
import Axios from 'axios';
import { ref } from 'vue';

export default () => {
    let isLoading = ref(true)
    let users = ref<ApplicationUser[]>()
    let usersClient = new UsersClient()
    let load = async () => {
        isLoading.value = true

        try {
            let res = await usersClient.getUsers(0, 10, "")
            users.value = res.rows
        } catch (ex) {
            console.log(ex)
        }
        isLoading.value = false
    }

    return {
        isLoading, users, load
    }
}
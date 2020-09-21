import { ApplicationUser, UsersClient } from './../api';
import Axios from 'axios';
import { ref } from 'vue';

export default () => {
    let isLoding = ref(true)
    let users = ref<ApplicationUser[]>()
    let usersClient = new UsersClient()
    let load = async () => {
        isLoding.value = true

        try {
            let res = await usersClient.getUsers(0, 10, "")
            users.value = res.rows
        } catch (ex) {
            console.log(ex)
        }
        isLoding.value = false
    }

    return {
        isLoding, users, load
    }
}
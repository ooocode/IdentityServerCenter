import { ApplicationUser, UsersClient } from './../api';
import { ref } from 'vue';

export default (userId:string) => {
    let isLoding = ref(true)
    let user = ref<ApplicationUser>()
    let usersClient = new UsersClient()

    let load = async () => {
        isLoding.value = true

        try {
            let res = await usersClient.getUserById(userId)
            user.value = res.rows
        } catch (ex) {
            console.log(ex)
        }
        isLoding.value = false
    }

    return {
        isLoding, users, load
    }
}
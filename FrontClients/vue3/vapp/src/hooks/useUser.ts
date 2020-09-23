import { ApplicationUser, CreateOrUpdateUserViewModel, UsersClient } from './../api';
import { ref } from 'vue';

export default (userId: string | undefined) => {
    let isLoading = ref(true)
    let user = ref<ApplicationUser>()
    let usersClient = new UsersClient()

    let Load = async () => {

        isLoading.value = true
        if (userId) {
            try {
                let res = await usersClient.getUserById(userId)
                user.value = res
            } catch (ex) {
                console.log(ex)
            }
        }

        if (!user.value) {
            user.value = new ApplicationUser()
        }


        isLoading.value = false
    }

    let CreateOrUpdate = async () => {
        let vm = new CreateOrUpdateUserViewModel()
        vm.id = user?.value?.id;
        vm.name = user?.value?.name ?? "";
        vm.userName = user?.value?.userName ?? ""
        vm.password = user?.value?.password ?? ""
        await usersClient.createOrUpdateUser(vm)
    }

    return {
        isLoading, user, Load, CreateOrUpdate
    }
}
import React, { useState, useEffect } from "react"
import { Link } from "gatsby"
import { UsersClient, ApplicationUser } from "../../../api"

import { MainLayout } from "../../components/MainLayout";


const useUsers = (loadingUsersOnFirst: boolean = true) => {
    let [users, setUsers] = useState<ApplicationUser[] | undefined>()
    let [reloadUsers, SetReloadUsers] = useState(loadingUsersOnFirst)
    let [pending, setPending] = useState(true)

    const usersClient = new UsersClient()

    useEffect(() => {
        async function loadUsers() {
            setPending(true)
            try {
                let users = await (await usersClient.getUsers(0, 10, "")).rows
                setUsers(users)
            } catch (ex) {
                showErrorMsgBox(ex)
            }

            SetReloadUsers(false)
            setPending(false)
        }

        if (reloadUsers) {
            loadUsers()
        }
    }, [reloadUsers])


    const DeleteUser = (userId: string) => {
        return usersClient.deleteUserById(userId)
    }

    const ReloadUsers = () => SetReloadUsers(true)

    return { pending, users, ReloadUsers, DeleteUser }
}


import { showErrorMsgBox } from "../../../utility";



export default () => {
    

    let state = useUsers()

    return <MainLayout>
       Yonghu
    </MainLayout>
}


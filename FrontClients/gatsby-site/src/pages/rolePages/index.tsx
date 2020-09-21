import React, { useState, useEffect } from "react"
import { PageProps, Link } from "gatsby"
import { UsersClient, ApplicationUser, ApplicationRole, RolesClient } from "../../../api"
import { MainLayout } from "../../components/MainLayout"


const useRoles = (loadingRolesOnFirst: boolean = true) => {
    let [roles, setRoles] = useState<ApplicationRole[] | undefined>()
    let [reloadRoles, setReloadRoles] = useState(loadingRolesOnFirst)
    let [pending, setPending] = useState(true)

    const rolesClient = new RolesClient()

    useEffect(() => {
        async function loadRoles() {
            setPending(true)

            var roles = await rolesClient.getRoles(0, 10, "")
            setRoles(roles.rows)

            setPending(false)
        }

        loadRoles()

    }, [reloadRoles])


    const DeleteRole = (id: string) => {
        return rolesClient.deleteRoleById(id)
    }

    const ReloadUsers = () => setReloadRoles(true)

    return { pending, roles, ReloadUsers, DeleteRole }
}






export default () => {

    let state = useRoles()

    return <MainLayout>

    </MainLayout>
}


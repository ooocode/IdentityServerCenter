import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser, ApplicationRole, RolesClient, ApiException } from "../../../api"

import queryStringParser from "../../../queryStringParser"
import { MainLayout } from "../../components/MainLayout"


const useRole = (id: string, isLoadRoleOnFirst: boolean = true) => {
    let [pending, setPending] = useState(true)
    let [role, setRole] = useState<ApplicationRole>();
    let [reloadRole, setReloadRole] = useState(isLoadRoleOnFirst)

    const rolesClient = new RolesClient()

    useEffect(() => {
        async function loadRole() {
            setPending(true)

            if (id) {
                try {
                    let role = await rolesClient.getRoleById(id)
                    setRole(role)
                } catch (err) {
                    if (err instanceof ApiException) {
                      
                    }
                }
            } else {
                let role = new ApplicationUser()
                setRole(role)
            }

            setPending(false)
        }

        if (reloadRole) {
            loadRole().then(() => {
                setReloadRole(false)
            })
        }
    }, [reloadRole])


    /**
     * 重新加载用户
     */
    let ReloadRole = () => {
        setReloadRole(true)
    }

    /**
     * 创建或者更新
     * @param cb 
     */
    let CreateOrUpdate = (cb: () => void = null) => {
        rolesClient.createOrUpdateRole(role).then(res => {
            if (cb) {
                cb();
            }
        })
    }

    return { pending, role, ReloadRole, CreateOrUpdate }
}



export default () => {
    var id = queryStringParser().id as string

    var roleState = useRole(id)

    let onFinish = (value) => {
        roleState.CreateOrUpdate(() => {
            roleState.ReloadRole()
        })
    }


    if (roleState.pending) {
        return <MainLayout>加载中......</MainLayout>
    } else {
        let role = roleState.role;
        return <MainLayout>
          
        </MainLayout>
    }
}
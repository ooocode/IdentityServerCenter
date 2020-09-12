import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser, CreateOrUpdateUserViewModel, ApplicationRole, RolesClient } from "../../../api"

import queryStringParser from "../../../queryStringParser"

import { TextField, MaskedTextField } from 'office-ui-fabric-react/lib/TextField';
import { Stack, IStackProps, IStackStyles } from 'office-ui-fabric-react/lib/Stack';
import { DefaultButton, PrimaryButton, IStackTokens } from 'office-ui-fabric-react';


const useRole = (id: string, isLoadRoleOnFirst: boolean = true) => {
    let [pending, setPending] = useState(true)

    let [role, setRole] = useState<ApplicationRole>();
    const rolesClient = new RolesClient()

    let [reloadRole, setReloadRole] = useState(isLoadRoleOnFirst)

    useEffect(() => {
        if (reloadRole) {
            if (id) {
                setPending(true)
                rolesClient.getRoleById(id).then(res => {
                    setRole(res)
                    setPending(false)
                    setReloadRole(false)
                })
            } else {
                if (role == null) {
                    role = new ApplicationUser()
                }
                setPending(false)
            }
        }
    }, [reloadRole])


    /**
     * 重新加载用户
     */
    let ReloadRole = () => {
        setReloadRole(true)
    }


    return { pending, role, ReloadRole }
}



export default () => {
    var id = queryStringParser().id as string

    var userState = useRole(id)

    if (userState.pending) {
        return <div>加载中......</div>
    } else {
        let role = userState.role;
        return <Stack>
            <TextField label="账号" defaultValue={role.name} onChange={(e, value) => role.name = value} />
            <TextField label="姓名" defaultValue={role.desc} onChange={(e, value) => role.desc = value} />


            <DefaultButton text="确定" onClick={(e) => {

            }} />
        </Stack>
    }
}
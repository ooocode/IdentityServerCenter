import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser, CreateOrUpdateUserViewModel } from "../../../api"

import queryStringParser from "../../../queryStringParser"

import { TextField, MaskedTextField } from 'office-ui-fabric-react/lib/TextField';
import { Stack, IStackProps, IStackStyles } from 'office-ui-fabric-react/lib/Stack';
import { DefaultButton, PrimaryButton, IStackTokens } from 'office-ui-fabric-react';
import { Layout } from "../../components/layout";


const useUpdateUser = (id: string) => {
    let [isLoading, setLoading] = useState(true)

    let [user, setUser] = useState<ApplicationUser>();
    const usersClient = new UsersClient()
    let [hadCreateOrUpdateUser, setHadCreateOrUpdateUser] = useState(true)

    useEffect(() => {
        if (hadCreateOrUpdateUser) {
            if (id) {
                setLoading(true)
                usersClient.getUserById(id).then(res => {
                    setUser(res)
                    setLoading(false)
                    setHadCreateOrUpdateUser(false)
                })
            } else {
                if (user == null) {
                    user = new ApplicationUser()
                }
                setLoading(false)
            }
        }
    }, [hadCreateOrUpdateUser])


    /**
     * 重新加载用户
     */
    let ReloadUser = () => {
        setHadCreateOrUpdateUser(true)
    }


    let Update = (cb: () => void) => {
        setLoading(true)
        console.log(user)
        let vm = new CreateOrUpdateUserViewModel();
        vm.id = user.id
        vm.name = user.name
        vm.userName = user.userName
        vm.password = user.password

        usersClient.createOrUpdateUser(vm).then(res => {
            setLoading(false)
            cb()
        }).catch(err => setLoading(false))
    }

    return { isLoading, user, Update, ReloadUser }
}



export default () => {
    var id = queryStringParser().id as string

    var userState = useUpdateUser(id)

    var userState1 = useUpdateUser(id)

    if (userState.isLoading) {
        return <Layout>加载中......</Layout>
    } else {
        let user = userState.user;
        return <Layout>
            <Stack>
                <TextField label="账号" defaultValue={user?.userName} onChange={(e, value) => user.userName = value} />
                <TextField label="姓名" defaultValue={user?.name} onChange={(e, value) => user.name = value} />
                <TextField label="密码" defaultValue={user?.password} onChange={(e, value) => user.password = value} />


                {
                    userState1.isLoading == false ? <TextField label="账号" defaultValue={userState1?.user?.userName} onChange={(e, value) => user.userName = value} /> : <label>加载中</label>
                }

                {/*  <TextField label="Disabled" disabled defaultValue="I am disabled" />
            <TextField label="Read-only" readOnly defaultValue="I am read-only" />
            <TextField label="Required " required />
            <TextField ariaLabel="Required without visible label" required />
            <TextField label="With error message" errorMessage="Error message" /> */}

                <DefaultButton text="确定" onClick={(e) => {
                    userState.Update(() => {
                        userState.ReloadUser()
                    })
                }} />
            </Stack></Layout>
    }
}
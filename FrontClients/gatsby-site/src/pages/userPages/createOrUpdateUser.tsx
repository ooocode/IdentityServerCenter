import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser, CreateOrUpdateUserViewModel } from "../../../api"

import queryStringParser from "../../../queryStringParser"

import { MainLayout } from "../../components/MainLayout";
import { Button, Checkbox, Form, Input } from "antd";


const useUser = (id: string) => {
    let [pending, setPending] = useState(true)

    let [user, setUser] = useState<ApplicationUser>();

    let [reloadUser, setReloadUser] = useState(true)

    const usersClient = new UsersClient()

    useEffect(() => {
        async function loadUser() {
            setPending(true)
            if (id) {
                var user = await usersClient.getUserById(id);
                setUser(user)
            } else {
                var user = new ApplicationUser()
                setUser(user)
            }

            setPending(false)
            setReloadUser(false)
        }

        loadUser()
    }, [reloadUser])


    /**
     * 重新加载用户
     */
    let ReloadUser = () => {
        setReloadUser(true)
    }


    let CreateOrUpdate = (cb: () => void) => {
        setPending(true)
        console.log(user)
        let vm = new CreateOrUpdateUserViewModel();
        vm.id = user.id
        vm.name = user.name
        vm.userName = user.userName
        vm.password = user.password

        usersClient.createOrUpdateUser(vm).then(res => {
            setPending(false)
            cb()
        }).catch(err => setPending(false))
    }

    return { pending, user, CreateOrUpdate, ReloadUser }
}



export default () => {
    var id = queryStringParser().id as string

    var userState = useUser(id)

    const onFinish = values => {
        console.log(userState.user)
        userState.CreateOrUpdate(() => {
            userState.ReloadUser()
        })
    };

    const onFinishFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };


    if (userState.pending) {
        return <MainLayout>加载中......</MainLayout>
    } else {
        return <MainLayout>
            <Form
                name="basic"
                initialValues={{ remember: true }}
                onFinish={onFinish}
                onFinishFailed={onFinishFailed}
            >
                <Form.Item
                    label="账号"
                    rules={[{ required: true, message: '请输入账号' }]}
                >
                    <Input onChange={(e) => userState.user.userName = e.target.value} />
                </Form.Item>


                <Form.Item
                    label="姓名"
                    rules={[{ required: true, message: '请输入姓名' }]}
                >
                    <Input onChange={(e) => userState.user.name = e.target.value} />
                </Form.Item>


                <Form.Item
                    label="密码"
                    rules={[{ required: true, message: '请输入密码' }]}
                >
                    <Input onChange={(e) => userState.user.password = e.target.value} />
                </Form.Item>

                <Form.Item>
                    <Button type="primary" htmlType="submit">
                        Submit
                    </Button>
                </Form.Item>
            </Form>
        </MainLayout>
    }
}
import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser, CreateOrUpdateUserViewModel } from "../../../api"

import queryStringParser from "../../../queryStringParser"

import { MainLayout } from "../../components/MainLayout";
import { Button, Checkbox, Form, Input } from "antd";
import { messageBox, showErrorMsgBox } from "../../../utility";


const useUser = (id: string) => {
    let [pending, setPending] = useState(true)

    let [user, setUser] = useState<ApplicationUser>();

    let [reloadUser, setReloadUser] = useState(true)

    const usersClient = new UsersClient()

    useEffect(() => {
        async function loadUser() {
            setPending(true)
            if (id) {
                //通过id查询
                var user = await usersClient.getUserById(id);
                setUser(user)
            } else {
                //新建
                var user = new ApplicationUser()
                setUser(user)
            }

            setPending(false)
            setReloadUser(false)
        }

        if (reloadUser) {
            loadUser()
        }
    }, [reloadUser])


    /**
     * 重新加载用户
     */
    let ReloadUser = () => {
        if (id) {
            setReloadUser(true)
        }
    }


    let CreateOrUpdate = (cb: () => void) => {
        console.log(user)
        let vm = new CreateOrUpdateUserViewModel();
        vm.id = user.id
        vm.name = user.name
        vm.userName = user.userName
        vm.password = user.password

        async function func() {
            setPending(true)
            try {
                await usersClient.createOrUpdateUser(vm)
                messageBox(id ? "更新用户成功" : "创建用户成功", "success")
                cb()
            } catch (ex) {
                showErrorMsgBox(ex)
            }

            setPending(false)
        }
        func()
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
        var user = userState.user
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
                    <Input onChange={(e) => user.userName = e.target.value} defaultValue={user.userName} />
                </Form.Item>


                <Form.Item
                    label="姓名"
                    rules={[{ required: true, message: '请输入姓名' }]}
                >
                    <Input onChange={(e) => user.name = e.target.value} defaultValue={user.name} />
                </Form.Item>


                <Form.Item
                    label="密码"
                    rules={[{ required: true, message: '请输入密码' }]}
                >
                    <Input onChange={(e) => user.password = e.target.value} defaultValue={user.password} />
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
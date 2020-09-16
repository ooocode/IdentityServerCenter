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



import { Button, Table } from 'antd';
import { ColumnsType } from "antd/lib/table";
import { showErrorMsgBox } from "../../../utility";



export default () => {
    const columns: ColumnsType<ApplicationUser> = [
        {
            title: '账号',
            width: 150,
            dataIndex: 'userName',
            key: "userName",
            fixed: 'left',
        },
        {
            title: '姓名',
            width: 150,
            dataIndex: 'name',
            key: "name",
            fixed: 'left',
        },
        {
            title: '邮箱',
            dataIndex: 'email',
            key: 'email',
            width: 150,
        },
        {
            title: '手机号码',
            dataIndex: 'phone',
            key: 'phone',
            width: 150,
        },
        {
            title: 'Action',
            key: 'operation',
            fixed: 'right',
            width: 100,
            render: (user: ApplicationUser) => <>
                <Link to={"/userPages/createOrUpdateUser?id=" + user.id}>编辑</Link>
            </>,
        },
    ];

    let state = useUsers()

    return <MainLayout>
        <Button><Link to="createOrUpdateUser/">新建用户</Link></Button>
        <Table columns={columns}
            dataSource={state.users}
            scroll={{ x: 1500, y: 300 }} />
    </MainLayout>
}


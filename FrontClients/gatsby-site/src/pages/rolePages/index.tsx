import React, { useState, useEffect } from "react"
import { PageProps, Link } from "gatsby"
import { UsersClient, ApplicationUser, ApplicationRole, RolesClient } from "../../../api"
import { MainLayout } from "../../components/MainLayout"
import { Button, Table } from 'antd';
import { ColumnsType } from "antd/lib/table";


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
    const columns: ColumnsType<ApplicationUser> = [
        {
            title: '角色名',
            width: 150,
            dataIndex: 'name',
            key: "name",
            fixed: 'left',
        },
        {
            title: '描述',
            width: 150,
            dataIndex: 'desc',
            key: "desc",
            fixed: 'left',
        },
        {
            title: 'Action',
            key: 'operation',
            fixed: 'right',
            width: 100,
            render: (role: ApplicationRole) => <>
                <Link to={"/rolePages/createOrUpdateRole?id=" + role.id}>编辑</Link>
            </>,
        },
    ];

    let state = useRoles()

    return <MainLayout>
        <Button><Link to="createOrUpdateRole/">新建角色</Link></Button>
        <Table columns={columns} dataSource={state.roles} scroll={{ x: 1500, y: 300 }} />

    </MainLayout>
}


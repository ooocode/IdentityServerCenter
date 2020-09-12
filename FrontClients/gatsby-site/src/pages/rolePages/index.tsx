import React, { useState, useEffect } from "react"
import { PageProps, Link } from "gatsby"
import { UsersClient, ApplicationUser, ApplicationRole, RolesClient } from "../../../api"

import {
    DetailsList,
    DetailsListLayoutMode,
    Selection,
    SelectionMode,
    IColumn,
    IDetailsListProps,
    DetailsRow,
} from 'office-ui-fabric-react/lib/DetailsList';

import { DefaultButton, PrimaryButton, Stack, IStackTokens } from 'office-ui-fabric-react';



const useRoles = (loadingRolesOnFirst: boolean = true) => {
    let [roles, setRoles] = useState<ApplicationRole[] | undefined>()
    let [reloadRoles, setReloadRoles] = useState(loadingRolesOnFirst)
    let [pending, setPending] = useState(true)

    const rolesClient = new RolesClient()

    useEffect(() => {
        if (reloadRoles) {
            setPending(true)
            rolesClient.getRoles(0, 10, "").then(res => {
                setRoles(res.rows)
                setReloadRoles(false)
                setPending(false)
            }).catch(err => {
                setReloadRoles(false)
                setPending(false)
            })
        }
    }, [reloadRoles])


    const DeleteRole = (id: string) => {
        return rolesClient.deleteRoleById(id)
    }

    const ReloadUsers = () => setReloadRoles(true)

    return { pending, roles, ReloadUsers, DeleteRole }
}

export default () => {
    let state = useRoles()

    const columns: IColumn[] = [
        {
            key: "用户名（登录账号）",
            name: '用户名（登录账号）',

            //   className: classNames.fileIconCell,
            //   iconClassName: classNames.fileIconHeaderIcon,
            //ariaLabel: 'Column operations for File type, Press to sort on File type',
            //iconName: 'Page',
            //isIconOnly: true,
            //fieldName: 'name',
            minWidth: 150,
            maxWidth: 200,
            //onColumnClick: this._onColumnClick,
            onRender: (item: ApplicationRole) => {
                return <div>{item.name}</div>;
            }
        },
        {
            key: '姓名',
            name: '姓名',
            //   className: classNames.fileIconCell,
            //   iconClassName: classNames.fileIconHeaderIcon,
            //iconName: 'Page',
            //isIconOnly: true,
            //fieldName: 'name',
            minWidth: 100,
            maxWidth: 150,
            //onColumnClick: this._onColumnClick,
            onRender: (item: ApplicationRole) => {
                return <div>{item.nonEditable}</div>;
            }
        },
        {
            key: '性别',
            name: '性别',
            //   className: classNames.fileIconCell,
            //   iconClassName: classNames.fileIconHeaderIcon,
            //ariaLabel: 'Column operations for File type, Press to sort on File type',
            //iconName: 'Page',
            //isIconOnly: true,
            //fieldName: 'name',
            minWidth: 100,
            maxWidth: 100,
            //onColumnClick: this._onColumnClick,
            onRender: (item: ApplicationRole) => {
                return <div>{item.desc}</div>;
            }
        },
        {
            key: '操作',
            name: '操作',
            //   className: classNames.fileIconCell,
            //   iconClassName: classNames.fileIconHeaderIcon,
            //ariaLabel: 'Column operations for File type, Press to sort on File type',
            //iconName: 'Page',
            //isIconOnly: true,
            //fieldName: 'name',
            minWidth: 16,
            //maxWidth: 16,
            //onColumnClick: this._onColumnClick,
            onRender: (role: ApplicationRole) => {
                return <div>
                    <DefaultButton><Link to={"createOrUpdateRole?id=" + role.id}>{role.name}</Link></DefaultButton>
                    <DefaultButton text="删除" onClick={(e) => {
                        state.DeleteRole(role.id).then(res => {
                            state.ReloadUsers();
                        })
                    }} />
                </div>
            }
        },
    ]

    return <div>
        {state.pending ? <label>加载中</label> : <DetailsList items={state.roles ?? []} columns={columns}></DetailsList>}
    </div>
}
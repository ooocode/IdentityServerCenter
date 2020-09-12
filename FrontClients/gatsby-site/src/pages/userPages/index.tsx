import React, { useState, useEffect } from "react"
import { PageProps, Link } from "gatsby"
import { UsersClient, ApplicationUser } from "../../../api"
import { Breadcrumb, IBreadcrumbItem, IDividerAsProps } from 'office-ui-fabric-react/lib/Breadcrumb';
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
import { MainLayout } from "../../components/MainLayout";



const useUsers = (loadingUsersOnFirst: boolean = true) => {
    let [users, setUsers] = useState<ApplicationUser[] | undefined>()
    let [reloadUsers, SetReloadUsers] = useState(loadingUsersOnFirst)
    let [pending, setPending] = useState(true)

    const usersClient = new UsersClient()

    useEffect(() => {
        if (reloadUsers) {
            setPending(true)
            usersClient.getUsers(0, 10, "").then(res => {
                setUsers(res.rows)
                SetReloadUsers(false)
                setPending(false)
            }).catch(err => {
                SetReloadUsers(false)
                setPending(false)
            })
        }
    }, [reloadUsers])


    const DeleteUser = (userId: string) => {
        return usersClient.deleteUserById(userId)
    }

    const ReloadUsers = () => SetReloadUsers(true)

    return { pending, users, ReloadUsers, DeleteUser }
}

export default () => {
    let state = useUsers()

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
            onRender: (item: ApplicationUser) => {
                return <div>{item.userName}</div>;
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
            onRender: (item: ApplicationUser) => {
                return <div>{item.name}</div>;
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
            onRender: (item: ApplicationUser) => {
                return <div>{item.sex == 0 ? "男" : "女"}</div>;
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
            onRender: (user: ApplicationUser) => {
                return <div>
                    <DefaultButton><Link to={"createOrUpdateUser?id=" + user.id}>{user.name}</Link></DefaultButton>
                    <DefaultButton text="删除" onClick={(e) => {
                        state.DeleteUser(user.id).then(res => {
                            state.ReloadUsers();
                        })
                    }} />
                </div>
            }
        },
    ]

    const items: IBreadcrumbItem[] = [
        { text: '主页', key: '1', href: "#/" },
        { text: '用户管理', key: '2', isCurrentItem: true },
    ];

    return <MainLayout>

        <Breadcrumb
            items={items}
            maxDisplayedItems={3}
            ariaLabel="Breadcrumb with items rendered as links"
            overflowAriaLabel="More links"
        />
        {state.pending ? <label>加载中</label> : <DetailsList items={state.users ?? []} columns={columns}></DetailsList>}
    </MainLayout>
}
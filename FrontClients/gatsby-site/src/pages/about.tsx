import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser } from "../../api"

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


let c = (str: number) => {
    return str
}
type QueryArgs = { id: number }

export default function about(props: PageProps<QueryArgs>) {

    let [users, setUsers] = useState<ApplicationUser[] | undefined>()
    useEffect(() => {
        let userClient = new UsersClient()
        userClient.getUsers(0, 10, "").then(res => {
            setUsers(res.rows)
        })

    }, [])


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
            onRender: (item: ApplicationUser) => {
                return <DefaultButton text="删除" onClick={(e) => {
                    UsersClient
                }} />

            }
        },
    ]

    return <div>
        <DetailsList items={users ?? []} columns={columns}></DetailsList>
    </div>
}
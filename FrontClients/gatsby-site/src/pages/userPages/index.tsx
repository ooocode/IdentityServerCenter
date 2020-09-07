import React, { useState, useEffect } from "react"
import { PageProps, Link } from "gatsby"
import { UsersClient, ApplicationUser } from "../../../api"



export default () => {

    let [users, setUsers] = useState<ApplicationUser[] | undefined>()
    useEffect(() => {
        let userClient = new UsersClient()
        userClient.getUsers(0, 10, "").then(res => {
            setUsers(res.rows)
        })

    }, [])


    return <div>
        <table>
            <tbody>
                {users?.map(user => {
                    return <tr key={user.id}>
                        <td>{user.id}</td>
                        <td>{user.name}</td>
                        <td><Link to={"createOrUpdateUser?id=" + user.id}>{user.name}</Link></td>
                    </tr>
                })}
            </tbody>
        </table>

    </div>
}
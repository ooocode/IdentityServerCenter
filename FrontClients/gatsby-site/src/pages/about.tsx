import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser } from "../../api"


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


    return <div>
        <div>111111</div>
    </div>
}
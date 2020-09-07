import React, { useState, useEffect } from "react"
import { PageProps } from "gatsby"
import { UsersClient, ApplicationUser } from "../../../api"

import queryStringParser from "../../../queryStringParser"


export default () => {

    var id = queryStringParser().id as string
    let [user, setUser] = useState<ApplicationUser>();
    useEffect(() => {
        var usersClient = new UsersClient()
        usersClient.getUserById(id).then(res => setUser(res))
    }, [])


    return <div>
        <input value={user?.name} onChange={(e)=>e.target.value} />
        1111
    </div>
}
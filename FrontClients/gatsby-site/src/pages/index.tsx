import React from "react"
import { Link } from "gatsby"

export default () => {
  return <>
    <Link to="/userPages/">用户</Link><br />
    <Link to="/rolePages/">角色</Link>
  </>
}
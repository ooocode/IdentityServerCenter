import React from "react"
import { Link } from "gatsby"
import { Layout } from "../components/layout"

export default () => {
  return <>
    <Layout>
      <Link to="/userPages/">用户</Link><br />
      <Link to="/rolePages/">角色</Link>
    </Layout>

  </>
}
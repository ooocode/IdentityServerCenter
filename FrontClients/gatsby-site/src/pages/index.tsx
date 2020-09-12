import React from "react"
import { Link } from "gatsby"
import { MainLayout } from "../components/MainLayout"

export default () => {
  return <>
    <MainLayout>
      <Link to="/userPages/">用户</Link><br />
      <Link to="/rolePages/">角色</Link>
    </MainLayout>

  </>
}
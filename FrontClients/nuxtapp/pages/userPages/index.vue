<template>
  <div>
    <nuxt-link to="createOrUpdateUser">新建</nuxt-link>

    <v-data-table
      v-model="selected"
      :headers="headers"
      :items="desserts"
      :single-select="singleSelect"
      item-key="id"
      show-select
      class="elevation-1"
    >
      <template v-slot:top>
        <v-switch v-model="singleSelect" label="Single select" class="pa-3"></v-switch>
      </template>
    </v-data-table>
  </div>
</template>


<script lang="ts">
import Logo from '~/components/Logo.vue'
import VuetifyLogo from '~/components/VuetifyLogo.vue'
import { UsersClient, ApplicationUser } from '../../api'

interface IData {
  usersClient: UsersClient
  singleSelect: boolean
  selected: ApplicationUser[]
  headers: any[]
  desserts: ApplicationUser[]
}

export default {
  data(): IData {
    return {
      usersClient: new UsersClient(),
      singleSelect: false,
      selected: [],
      headers: [
        {
          text: '用户名',
          align: 'start',
          sortable: false,
          value: 'userName',
        },
        { text: '姓名', value: 'name' },
        { text: '邮箱', value: 'email' },
        { text: '操作' },
      ],
      desserts: [],
    }
  },

  created() {
    let usersClient = new UsersClient()
    usersClient.getUsers(0, 10, '').then((res) => {
      
    })
  },
}
</script>

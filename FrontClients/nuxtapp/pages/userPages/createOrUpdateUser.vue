<template>
  <v-form v-model="valid">
    <v-container>
      <v-row>
        <v-col cols="12" md="4">
          <v-text-field v-model="CreateOrUpdateUserViewModel.userName" label="First name" required></v-text-field>
        </v-col>

        <v-col cols="12" md="4">
          <v-text-field v-model="CreateOrUpdateUserViewModel.name" label="Last name" required></v-text-field>
        </v-col>

        <v-col cols="12" md="4">
          <v-text-field v-model="CreateOrUpdateUserViewModel.password" required></v-text-field>
        </v-col>
      </v-row>

      <v-btn @click="submit">提交</v-btn>
    </v-container>
  </v-form>
</template>

<script lang="ts">
import { CreateOrUpdateUserViewModel, UsersClient } from '../../api'
interface IData {
  valid: boolean
  UsersClient: UsersClient
  CreateOrUpdateUserViewModel: CreateOrUpdateUserViewModel
}
export default {
  data: (): IData => ({
    valid: false,
    UsersClient: new UsersClient(),
    CreateOrUpdateUserViewModel: CreateOrUpdateUserViewModel.fromJS({}),
  }),
  created() {},
  methods: {
    submit() {
      console.log(this.CreateOrUpdateUserViewModel)
      try {
        this.UsersClient.createOrUpdateUser(this.CreateOrUpdateUserViewModel)
      } catch (ex) {}
    },
  },
}
</script>
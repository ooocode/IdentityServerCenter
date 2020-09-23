<template>
  <div>
    <router-link class="btn btn-primary btn-sm" :to="getEditLink('')">新建</router-link>
    <table class="table">
      <thead>
        <tr>
          <td>账号</td>
          <td>姓名</td>
        </tr>
      </thead>

      <tbody>
        <tr v-for="user in users.users.value" :key="user.id">
          <td>{{user.userName}}</td>
          <td>{{user.name}}</td>
          <td>
            <router-link class="btn btn-primary btn-sm" :to="this.getEditLink(user.id)">编辑</router-link>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { onMounted } from "vue";
import useUsers from "../../hooks/useUsers";
export default {
  setup() {
    let users = useUsers();
    onMounted(async () => {
      await users.load();
      console.log(users.users.value);
    });

    return { users };
  },

  methods: {
    getEditLink(id: string) {
      return "/UserPages/CreateOrUpdateUser?id=" + id;
    },
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>

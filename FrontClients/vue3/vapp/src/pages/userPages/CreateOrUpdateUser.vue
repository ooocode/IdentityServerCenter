<template>
  <div>
    <label for>用户详情页</label>
    <div v-if="!userState.isLoading.value">
      <div class="form-group">
        <label for>用户名</label>
        <input type="text" class="form-control" v-model="userState.user.value.userName" />
      </div>

      <div class="form-group">
        <label for>姓名</label>
        <input type="text" class="form-control" v-model="userState.user.value.name" />
      </div>

      <div class="form-group">
        <label for>密码</label>
        <input type="text" class="form-control" v-model="userState.user.value.password" />
      </div>

      <div class="form-group">
        <button class="btn btn-primary btn-sm" @click="btnSaveClicked">保存</button>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import useUser from "@/hooks/useUser";
import { useRoute } from "vue-router";
import { onMounted } from "vue";
export default {
  setup() {
    let route = useRoute();
    let userState = useUser(route.query.id as string);

    onMounted(async () => {
      await userState.Load();
      console.log(userState.user.value);
    });

    let btnSaveClicked = async () => {
      await userState.CreateOrUpdate();
      await userState.Load();
    };

    return {
      userState,
      btnSaveClicked,
    };
  },
};
</script>

<style>
</style>
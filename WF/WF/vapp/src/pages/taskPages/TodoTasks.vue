<template>
  <div>
    <table class="table">
      <thead>
        <tr>
          <th>标题</th>
          <th>文号</th>
          <th>收件人</th>
          <th>当前状态</th>
          <th>发件时间</th>
        </tr>
      </thead>


      <tbody v-if="todos.isLoading.value===false">
        <tr v-for="task in todos.todoTasks.value" :key="task.taskId">
          <td>【{{task.arch?.processDefinitionName}}】{{task.arch?.title}}</td>
          <td>{{task.arch?.archNo}}</td>
          <td>{{task.arch?.userName}}</td>
          <td>{{task.task?.flowStatus}}</td>
          <td>{{task.task?.createTime}}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import useTodoTasks from "../../hooks/useTodoTasks";
import { onMounted } from "vue";
export default {
  setup() {
    const todos = useTodoTasks();

    onMounted(async () => {
      await todos.load();
      console.log(todos.todoTasks);
    });

    return {
      todos,
    };
  },
};
</script>

<style>
</style>
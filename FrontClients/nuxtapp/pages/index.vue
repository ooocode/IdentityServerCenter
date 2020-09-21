<template>
  <div>
    <h5>首页</h5>
    <v-list>
      <v-list-item v-for="item in processDefinitions" :key="item.id">
        <v-btn @click="startProcess(item.id)">{{item.name}}</v-btn>
      </v-list-item>
    </v-list>
  </div>
</template>


<script lang="ts">
import Logo from '~/components/Logo.vue'
import VuetifyLogo from '~/components/VuetifyLogo.vue'
import { UsersClient, ApplicationUser } from '../api'
import { TasksService, ProcessDefinitionModel } from '../bmpn'

interface IData {
  tasksService: TasksService
  processDefinitions: ProcessDefinitionModel[] | undefined
}
export default {
  data(): IData {
    return {
      tasksService: new TasksService(),
      processDefinitions: [],
    }
  },

  created() {
    this.tasksService.getProcessDefinitions().then((res) => {
      console.log(res)
      this.processDefinitions = res
    })
  },

  methods: {
    startProcess(id: string) {
      this.tasksService.startProcessDefinitionById(id).then((id) => {
        alert(id)
      })
    },
  },
}
</script>

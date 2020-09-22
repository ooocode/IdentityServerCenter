import TodoTasks from './pages/taskPages/TodoTasks.vue'
import DoneTasks from './pages/taskPages/DoneTasks.vue'
export const routes = [
    { path: '/', component: TodoTasks },
    { path: '/DoneTasks', component: DoneTasks },
    //{ path: '/about', component: About },
]

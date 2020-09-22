import { createApp } from 'vue'
import App from './App.vue'
import { useRouter, createRouter, createWebHistory } from 'vue-router'
import { routes } from './routers';
// 2. Define some routes
// Each route should map to a component.
// We'll talk about nested routes later.

// 3. Create the router instance and pass the `routes` option
// You can pass in additional options here, but let's
// keep it simple for now.
const router = createRouter({
    history: createWebHistory(),
    routes
});

useRouter()


const app = createApp(App)
app.use(router)
app.mount('#app')

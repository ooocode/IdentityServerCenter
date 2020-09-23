/* import UserIndex from './pages/userPages/UserIndex.vue'


import UserIndex from './pages/userPages/UserIndex.vue'
import CreateOrUpdateUser from './pages/userPages/CreateOrUpdateUser.vue' */
import CreateOrUpdateUser from './pages/userPages/CreateOrUpdateUser.vue'

export const routes = [
    { path: '/', component: () => require("./pages/userPages/UserIndex.vue") },
    { path: '/UserPages/CreateOrUpdateUser', component: CreateOrUpdateUser },

    { path: '/rolePages/RoleIndex', component: () => require("./pages/rolePages/RoleIndex.vue") },

    //{ path: '/createOrUpdateUser', component: CreateOrUpdateUser },


    //{ path: '/', component: Index },
    //{ path: '/createOrUpdateUser', component: Index },
    //{ path: '/about', component: About },
]

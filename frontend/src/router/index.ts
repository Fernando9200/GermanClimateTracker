import { createRouter, createWebHistory } from 'vue-router'
import DataGrid from '../components/DataGrid.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: DataGrid
    }
  ]
})

export default router
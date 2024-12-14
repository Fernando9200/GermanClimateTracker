import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'
import router from './router'  // Make sure this exists

const app = createApp(App)
app.use(ElementPlus)
app.use(router)    // Add the router
app.mount('#app')
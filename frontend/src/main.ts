import { createApp } from 'vue'
import { createPinia } from 'pinia'
import './style.css'
import App from './App.vue'
import router from './router/rutas'

const aplicacion = createApp(App)

aplicacion.use(createPinia())
aplicacion.use(router)

aplicacion.mount('#app')

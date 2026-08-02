import { createRouter, createWebHistory } from "vue-router"
import LoginVista from "../views/LoginVista.vue"
import SolicitudesVista from "../views/SolicitudesVista.vue"
import FormularioSolicitudVista from "../views/FormularioSolicitudVista.vue"
import DetalleSolicitudVista from "../views/DetalleSolicitudVista.vue"
import { useSesionStore } from "../stores/sesion"

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/login",
      component: LoginVista,
    },
    {
      path: "/solicitudes",
      component: SolicitudesVista,
      meta: { privada: true },
    },
    {
      path: "/solicitudes/nueva",
      component: FormularioSolicitudVista,
      meta: { privada: true },
    },
    {
      path: "/solicitudes/:id/editar",
      component: FormularioSolicitudVista,
      meta: { privada: true },
    },
    {
      path: "/solicitudes/:id",
      component: DetalleSolicitudVista,
      meta: { privada: true },
    },
  ],
})

router.beforeEach((ruta) => {
  const sesion = useSesionStore()

  if (ruta.meta.privada && !sesion.sesionIniciada) {
    return "/login"
  }

  return true
})

export default router

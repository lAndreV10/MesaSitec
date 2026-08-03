<script setup lang="ts">
import NavegacionAplicacion from "./components/NavegacionAplicacion.vue"
import { useNotificacionStore } from "./stores/notificacion"
import { useSesionStore } from "./stores/sesion"

const sesion = useSesionStore()
const notificacion = useNotificacionStore()
</script>

<template>
  <NavegacionAplicacion v-if="sesion.sesionIniciada" />
  <p v-if="notificacion.mensaje" data-testid="toast-mensaje" class="toast-mensaje">
    {{ notificacion.mensaje }}
  </p>
  <RouterView />
</template>

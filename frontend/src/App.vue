<script setup lang="ts">
import NavegacionAplicacion from "./components/NavegacionAplicacion.vue"
import { useSesionStore } from "./stores/sesion"

const sesion = useSesionStore()
</script>

<template>
  <NavegacionAplicacion v-if="sesion.sesionIniciada" />
  <RouterView />
</template>

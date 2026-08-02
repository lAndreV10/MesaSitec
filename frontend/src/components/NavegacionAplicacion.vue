<script setup lang="ts">
import { useRouter } from "vue-router"
import { useSesionStore } from "../stores/sesion"

const router = useRouter()
const sesion = useSesionStore()

function cerrarSesion() {
  sesion.cerrarSesion()
  router.push("/login")
}
</script>

<template>
  <nav data-testid="app-nav">
    <span data-testid="nav-usuario-nombre">
      {{ sesion.usuario?.nombre }}
    </span>

    <span data-testid="nav-usuario-rol">
      {{ sesion.usuario?.rol }}
    </span>

    <button data-testid="btn-logout" type="button" @click="cerrarSesion">
      Cerrar sesión
    </button>
  </nav>
</template>

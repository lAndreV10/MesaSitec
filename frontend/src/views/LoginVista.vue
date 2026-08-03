<script setup lang="ts">
import { ref } from "vue"
import { useRouter } from "vue-router"
import { useNotificacionStore } from "../stores/notificacion"
import { useSesionStore } from "../stores/sesion"

const email = ref("")
const password = ref("")
const error = ref("")
const cargando = ref(false)

const router = useRouter()
const sesion = useSesionStore()
const notificacion = useNotificacionStore()

async function enviarFormulario() {
  error.value = ""
  cargando.value = true

  try {
    await sesion.iniciarSesion({
      email: email.value,
      password: password.value,
    })

    await router.push("/solicitudes")
    notificacion.mostrar("Sesión iniciada.")
  } catch (errorCapturado) {
    error.value = errorCapturado instanceof Error
      ? errorCapturado.message
      : "No fue posible iniciar sesión."
    notificacion.mostrar(error.value)
  } finally {
    cargando.value = false
  }
}
</script>

<template>
  <main class="vista-login">
    <form class="tarjeta formulario" @submit.prevent="enviarFormulario">
      <label>
        Correo electrónico
        <input
          v-model="email"
          data-testid="login-email"
          type="email"
          :disabled="cargando"
          required
        />
      </label>

      <label>
        Contraseña
        <input
          v-model="password"
          data-testid="login-password"
          type="password"
          :disabled="cargando"
          required
        />
      </label>

      <p v-if="error" data-testid="login-error">
        {{ error }}
      </p>

      <button data-testid="login-submit" type="submit" :disabled="cargando">
        {{ cargando ? "Ingresando..." : "Ingresar" }}
      </button>
    </form>
  </main>
</template>

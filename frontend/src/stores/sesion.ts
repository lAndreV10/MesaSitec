import { computed, ref } from "vue"
import { defineStore } from "pinia"
import { peticion } from "../api/clienteHttp"
import type {
  InicioSesion,
  ResultadoInicioSesion,
  UsuarioSesion,
} from "../types/autenticacion"

const CLAVE_TOKEN = "accessToken"
const CLAVE_USUARIO = "usuario"

export const useSesionStore = defineStore("sesion", () => {
  const token = ref<string | null>(localStorage.getItem(CLAVE_TOKEN))
  const usuarioGuardado = localStorage.getItem(CLAVE_USUARIO)
  const usuario = ref<UsuarioSesion | null>(
    usuarioGuardado ? (JSON.parse(usuarioGuardado) as UsuarioSesion) : null,
  )

  const sesionIniciada = computed(() => token.value !== null && usuario.value !== null)

  function guardarSesion(nuevoToken: string, nuevoUsuario: UsuarioSesion) {
    token.value = nuevoToken
    usuario.value = nuevoUsuario

    localStorage.setItem(CLAVE_TOKEN, nuevoToken)
    localStorage.setItem(CLAVE_USUARIO, JSON.stringify(nuevoUsuario))
  }

  async function iniciarSesion(datos: InicioSesion) {
    const resultado = await peticion<ResultadoInicioSesion>("/auth/login", {
      method: "POST",
      body: JSON.stringify(datos),
    })

    guardarSesion(resultado.accessToken, resultado.usuario)
  }

  function cerrarSesion() {
    token.value = null
    usuario.value = null

    localStorage.removeItem(CLAVE_TOKEN)
    localStorage.removeItem(CLAVE_USUARIO)
  }

  return {
    usuario,
    sesionIniciada,
    iniciarSesion,
    cerrarSesion,
  }
})

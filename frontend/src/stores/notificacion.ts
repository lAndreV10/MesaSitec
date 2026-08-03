import { ref } from "vue"
import { defineStore } from "pinia"

export const useNotificacionStore = defineStore("notificacion", () => {
  const mensaje = ref("")
  let temporizador: ReturnType<typeof setTimeout> | undefined

  function mostrar(nuevoMensaje: string) {
    mensaje.value = nuevoMensaje

    if (temporizador) clearTimeout(temporizador)
    temporizador = setTimeout(() => {
      mensaje.value = ""
    }, 4000)
  }

  return { mensaje, mostrar }
})

<script setup lang="ts">
import { computed, onMounted, reactive, ref } from "vue"
import { useRoute, useRouter } from "vue-router"
import {
  crearSolicitud,
  editarSolicitud,
  obtenerCategorias,
  obtenerSolicitud,
} from "../api/solicitudesApi"
import { useNotificacionStore } from "../stores/notificacion"
import type { Categoria, DatosSolicitud } from "../types/solicitudes"

const ruta = useRoute()
const router = useRouter()
const notificacion = useNotificacionStore()

const categorias = ref<Categoria[]>([])
const cargando = ref(false)
const enviando = ref(false)
const errorGeneral = ref("")
const errorTitulo = ref("")
const errorDescripcion = ref("")
const errorCategoria = ref("")

const datos = reactive<DatosSolicitud>({
  titulo: "",
  descripcion: "",
  categoriaId: "",
  prioridad: "Media",
})

const idSolicitud = computed(() => String(ruta.params.id ?? ""))
const esEdicion = computed(() => idSolicitud.value.length > 0)

function validarFormulario() {
  errorTitulo.value = ""
  errorDescripcion.value = ""
  errorCategoria.value = ""

  if (datos.titulo.trim().length < 5 || datos.titulo.trim().length > 120) {
    errorTitulo.value = "El título debe tener entre 5 y 120 caracteres."
  }

  if (datos.descripcion.trim().length < 10 || datos.descripcion.trim().length > 4000) {
    errorDescripcion.value = "La descripción debe tener entre 10 y 4000 caracteres."
  }

  if (!datos.categoriaId) {
    errorCategoria.value = "Debes seleccionar una categoría."
  }

  return !errorTitulo.value && !errorDescripcion.value && !errorCategoria.value
}

async function cargarDatos() {
  cargando.value = true
  errorGeneral.value = ""

  try {
    categorias.value = await obtenerCategorias()

    if (esEdicion.value) {
      const solicitud = await obtenerSolicitud(idSolicitud.value)
      datos.titulo = solicitud.titulo
      datos.descripcion = solicitud.descripcion
      datos.categoriaId = solicitud.categoria.id
      datos.prioridad = solicitud.prioridad
    }
  } catch (error) {
    errorGeneral.value = error instanceof Error ? error.message : "No fue posible cargar el formulario."
  } finally {
    cargando.value = false
  }
}

async function guardarSolicitud() {
  if (!validarFormulario()) return

  enviando.value = true
  errorGeneral.value = ""

  try {
    const mensajeExito = esEdicion.value ? "Solicitud actualizada." : "Solicitud creada."
    const solicitud = esEdicion.value
      ? await editarSolicitud(idSolicitud.value, datos)
      : await crearSolicitud(datos)

    await router.push(`/solicitudes/${solicitud.id}`)
    notificacion.mostrar(mensajeExito)
  } catch (error) {
    errorGeneral.value = error instanceof Error ? error.message : "No fue posible guardar la solicitud."
    notificacion.mostrar(errorGeneral.value)
  } finally {
    enviando.value = false
  }
}

function cancelar() {
  router.push("/solicitudes")
}

onMounted(cargarDatos)
</script>

<template>
  <main class="vista-formulario">
    <h1>{{ esEdicion ? "Editar solicitud" : "Nueva solicitud" }}</h1>

    <p v-if="cargando">Cargando formulario...</p>
    <p v-else-if="errorGeneral">{{ errorGeneral }}</p>

    <form v-else class="tarjeta formulario" @submit.prevent="guardarSolicitud">
      <label>
        Título
        <input v-model="datos.titulo" data-testid="form-titulo" maxlength="120" />
      </label>
      <p v-if="errorTitulo" data-testid="error-titulo">{{ errorTitulo }}</p>

      <label>
        Descripción
        <textarea v-model="datos.descripcion" data-testid="form-descripcion" maxlength="4000" />
      </label>
      <p v-if="errorDescripcion" data-testid="error-descripcion">{{ errorDescripcion }}</p>

      <label>
        Categoría
        <select v-model="datos.categoriaId" data-testid="form-categoria">
          <option value="">Selecciona una categoría</option>
          <option v-for="categoria in categorias" :key="categoria.id" :value="categoria.id">
            {{ categoria.nombre }}
          </option>
        </select>
      </label>
      <p v-if="errorCategoria" data-testid="error-categoria">{{ errorCategoria }}</p>

      <label>
        Prioridad
        <select v-model="datos.prioridad" data-testid="form-prioridad">
          <option value="Baja">Baja</option>
          <option value="Media">Media</option>
          <option value="Alta">Alta</option>
          <option value="Critica">Crítica</option>
        </select>
      </label>

      <button type="submit" data-testid="form-submit" :disabled="enviando">
        {{ enviando ? "Guardando..." : "Guardar solicitud" }}
      </button>
      <button type="button" data-testid="form-cancelar" @click="cancelar">Cancelar</button>
    </form>
  </main>
</template>

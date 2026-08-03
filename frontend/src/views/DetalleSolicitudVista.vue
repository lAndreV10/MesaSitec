<script setup lang="ts">
import { computed, onMounted, ref } from "vue"
import { useRoute, useRouter } from "vue-router"
import {
  ejecutarTransicion,
  obtenerSolicitud,
  obtenerSolicitudes,
} from "../api/solicitudesApi"
import { useNotificacionStore } from "../stores/notificacion"
import { useSesionStore } from "../stores/sesion"
import type {
  AccionTransicion,
  DatosTransicion,
  SolicitudDetalle,
  UsuarioSolicitud,
} from "../types/solicitudes"

const ruta = useRoute()
const router = useRouter()
const sesion = useSesionStore()
const notificacion = useNotificacionStore()

const solicitud = ref<SolicitudDetalle | null>(null)
const cargando = ref(false)
const error = ref("")
const agentes = ref<UsuarioSolicitud[]>([])
const accionActual = ref<AccionTransicion | null>(null)
const agenteId = ref("")
const motivo = ref("")
const errorModal = ref("")
const procesandoAccion = ref(false)

const idSolicitud = computed(() => String(ruta.params.id))

const puedeEditar = computed(() => {
  if (!solicitud.value || !sesion.usuario) return false

  if (sesion.usuario.rol === "Admin" || sesion.usuario.rol === "Agente") {
    return true
  }

  return solicitud.value.solicitante.id === sesion.usuario.id && solicitud.value.estado === "Nueva"
})

const accionesDisponibles = computed<AccionTransicion[]>(() => {
  if (!solicitud.value || !sesion.usuario) return []

  const rol = sesion.usuario.rol
  const propia = solicitud.value.solicitante.id === sesion.usuario.id
  const esGestor = rol === "Admin" || rol === "Agente"
  const accionesPorEstado: Record<string, AccionTransicion[]> = {
    Nueva: ["asignar", "cancelar"],
    Asignada: ["asignar", "iniciar", "cancelar"],
    EnProceso: ["asignar", "resolver", "cancelar"],
    Resuelta: ["cerrar", "reabrir"],
    Cerrada: [],
    Cancelada: [],
  }

  return accionesPorEstado[solicitud.value.estado].filter((accion) => {
    if (accion === "cancelar") return rol === "Admin"
    if (accion === "cerrar") return esGestor || propia
    return esGestor
  })
})

async function cargarSolicitud() {
  cargando.value = true
  error.value = ""

  try {
    solicitud.value = await obtenerSolicitud(idSolicitud.value)
  } catch (errorCapturado) {
    error.value = errorCapturado instanceof Error
      ? errorCapturado.message
      : "No fue posible cargar la solicitud."
  } finally {
    cargando.value = false
  }
}

async function cargarAgentes() {
  if (!sesion.usuario || (sesion.usuario.rol !== "Admin" && sesion.usuario.rol !== "Agente")) {
    return
  }

  const listado = await obtenerSolicitudes({ page: 1, pageSize: 100 })
  const encontrados = listado.items
    .map((item) => item.agente)
    .filter((agente): agente is UsuarioSolicitud => agente !== null)

  if (!encontrados.some((agente) => agente.id === sesion.usuario?.id)) {
    encontrados.push({ id: sesion.usuario.id, nombre: sesion.usuario.nombre })
  }

  agentes.value = [...new Map(encontrados.map((agente) => [agente.id, agente])).values()]
  agenteId.value = sesion.usuario.id
}

function irAEditar() {
  router.push(`/solicitudes/${idSolicitud.value}/editar`)
}

function abrirAccion(accion: AccionTransicion) {
  accionActual.value = accion
  motivo.value = ""
  errorModal.value = ""
}

function cerrarModal() {
  accionActual.value = null
  errorModal.value = ""
}

async function confirmarAccion() {
  if (!accionActual.value || !solicitud.value) return

  errorModal.value = ""

  if (accionActual.value === "asignar" && !agenteId.value) {
    errorModal.value = "Selecciona un agente."
    return
  }

  if (accionActual.value === "resolver" && motivo.value.trim().length < 20) {
    errorModal.value = "El motivo debe tener al menos 20 caracteres."
    return
  }

  if (accionActual.value === "cancelar" && motivo.value.trim().length < 10) {
    errorModal.value = "El motivo debe tener al menos 10 caracteres."
    return
  }

  const datos: DatosTransicion = { accion: accionActual.value }
  if (accionActual.value === "asignar") datos.agenteId = agenteId.value
  if (accionActual.value === "resolver" || accionActual.value === "cancelar") {
    datos.motivo = motivo.value.trim()
  }

  procesandoAccion.value = true
  try {
    solicitud.value = await ejecutarTransicion(solicitud.value.id, datos)
    cerrarModal()
    notificacion.mostrar("Acción aplicada.")
  } catch (errorCapturado) {
    errorModal.value = errorCapturado instanceof Error
      ? errorCapturado.message
      : "No fue posible aplicar la acción."
    notificacion.mostrar(errorModal.value)
  } finally {
    procesandoAccion.value = false
  }
}

onMounted(async () => {
  await cargarSolicitud()
  try {
    await cargarAgentes()
  } catch {
    // La solicitud se puede consultar aunque no sea posible obtener agentes para asignar.
  }
})
</script>

<template>
  <main class="vista-detalle">
    <RouterLink to="/solicitudes">Volver al listado</RouterLink>

    <p v-if="cargando">Cargando solicitud...</p>
    <p v-else-if="error">{{ error }}</p>

    <section v-else-if="solicitud" class="tarjeta detalle-solicitud">
      <h1 data-testid="detalle-codigo">{{ solicitud.codigo }}</h1>
      <h2 data-testid="detalle-titulo">{{ solicitud.titulo }}</h2>
      <p data-testid="detalle-descripcion">{{ solicitud.descripcion }}</p>

      <dl>
        <dt>Estado</dt>
        <dd data-testid="detalle-estado">{{ solicitud.estado }}</dd>

        <dt>Prioridad</dt>
        <dd data-testid="detalle-prioridad">{{ solicitud.prioridad }}</dd>

        <dt>Categoría</dt>
        <dd data-testid="detalle-categoria">{{ solicitud.categoria.nombre }}</dd>

        <dt>Agente asignado</dt>
        <dd data-testid="detalle-agente">{{ solicitud.agente?.nombre ?? "Sin asignar" }}</dd>

        <dt>Fecha de creación</dt>
        <dd data-testid="detalle-fecha-creacion">{{ solicitud.fechaCreacion }}</dd>

        <dt>Fecha límite de SLA</dt>
        <dd data-testid="detalle-fecha-limite">{{ solicitud.fechaLimiteSla }}</dd>

        <dt v-if="solicitud.vencida">SLA</dt>
        <dd v-if="solicitud.vencida" data-testid="detalle-vencida">Vencida</dd>

        <dt v-if="solicitud.motivoResolucion || solicitud.motivoCancelacion">Motivo</dt>
        <dd v-if="solicitud.motivoResolucion || solicitud.motivoCancelacion" data-testid="detalle-motivo">
          {{ solicitud.motivoResolucion ?? solicitud.motivoCancelacion }}
        </dd>
      </dl>

      <button v-if="puedeEditar" data-testid="btn-editar" type="button" @click="irAEditar">
        Editar
      </button>

      <section v-if="accionesDisponibles.length > 0">
        <button
          v-for="accion in accionesDisponibles"
          :key="accion"
          :data-testid="`btn-accion-${accion}`"
          type="button"
          @click="abrirAccion(accion)"
        >
          {{ accion }}
        </button>
      </section>

      <section v-if="accionActual" data-testid="modal-accion">
        <h2>Confirmar: {{ accionActual }}</h2>

        <label v-if="accionActual === 'asignar'">
          Agente
          <select v-model="agenteId" data-testid="modal-select-agente">
            <option value="">Selecciona un agente</option>
            <option v-for="agente in agentes" :key="agente.id" :value="agente.id">
              {{ agente.nombre }}
            </option>
          </select>
        </label>

        <label v-if="accionActual === 'resolver' || accionActual === 'cancelar'">
          Motivo
          <textarea v-model="motivo" data-testid="modal-motivo" />
        </label>

        <p v-if="errorModal" data-testid="modal-error">{{ errorModal }}</p>

        <button data-testid="modal-confirmar" type="button" :disabled="procesandoAccion" @click="confirmarAccion">
          {{ procesandoAccion ? "Procesando..." : "Confirmar" }}
        </button>
        <button data-testid="modal-cancelar" type="button" @click="cerrarModal">Cancelar</button>
      </section>
    </section>
  </main>
</template>

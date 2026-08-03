<script setup lang="ts">
import { onMounted, ref } from "vue"
import { obtenerCategorias, obtenerSolicitudes } from "../api/solicitudesApi"
import type {
  Categoria,
  EstadoSolicitud,
  ListadoSolicitudes,
  PrioridadSolicitud,
} from "../types/solicitudes"

const categorias = ref<Categoria[]>([])
const listado = ref<ListadoSolicitudes>({
  items: [],
  page: 1,
  pageSize: 20,
  total: 0,
  totalPaginas: 0,
})

const estado = ref<EstadoSolicitud | "">("")
const prioridad = ref<PrioridadSolicitud | "">("")
const categoriaId = ref("")
const vencidas = ref(false)
const busqueda = ref("")
const cargando = ref(false)
const error = ref("")

async function cargarSolicitudes() {
  cargando.value = true
  error.value = ""

  try {
    listado.value = await obtenerSolicitudes({
      estado: estado.value || undefined,
      prioridad: prioridad.value || undefined,
      categoriaId: categoriaId.value || undefined,
      q: busqueda.value.trim() || undefined,
      vencidas: vencidas.value ? true : undefined,
      page: listado.value.page,
      pageSize: listado.value.pageSize,
      sort: "-fechaCreacion",
    })
  } catch (errorCapturado) {
    error.value = errorCapturado instanceof Error
      ? errorCapturado.message
      : "No fue posible cargar las solicitudes."
  } finally {
    cargando.value = false
  }
}

async function cargarCategorias() {
  try {
    categorias.value = await obtenerCategorias()
  } catch (errorCapturado) {
    error.value = errorCapturado instanceof Error
      ? errorCapturado.message
      : "No fue posible cargar las categorías."
  }
}

async function aplicarFiltros() {
  listado.value.page = 1
  await cargarSolicitudes()
}

async function limpiarFiltros() {
  estado.value = ""
  prioridad.value = ""
  categoriaId.value = ""
  vencidas.value = false
  busqueda.value = ""

  await aplicarFiltros()
}

async function paginaAnterior() {
  if (listado.value.page > 1) {
    listado.value.page -= 1
    await cargarSolicitudes()
  }
}

async function paginaSiguiente() {
  if (listado.value.page < listado.value.totalPaginas) {
    listado.value.page += 1
    await cargarSolicitudes()
  }
}

onMounted(async () => {
  await Promise.all([cargarCategorias(), cargarSolicitudes()])
})
</script>

<template>
  <main class="vista-listado">
    <RouterLink data-testid="btn-nueva-solicitud" to="/solicitudes/nueva">
      Nueva solicitud
    </RouterLink>

    <form class="filtros" @submit.prevent="aplicarFiltros">
      <select v-model="estado" data-testid="filtro-estado" @change="aplicarFiltros">
        <option value="">Todos los estados</option>
        <option value="Nueva">Nueva</option>
        <option value="Asignada">Asignada</option>
        <option value="EnProceso">En proceso</option>
        <option value="Resuelta">Resuelta</option>
        <option value="Cerrada">Cerrada</option>
        <option value="Cancelada">Cancelada</option>
      </select>

      <select v-model="prioridad" data-testid="filtro-prioridad" @change="aplicarFiltros">
        <option value="">Todas las prioridades</option>
        <option value="Baja">Baja</option>
        <option value="Media">Media</option>
        <option value="Alta">Alta</option>
        <option value="Critica">Crítica</option>
      </select>

      <select v-model="categoriaId" data-testid="filtro-categoria" @change="aplicarFiltros">
        <option value="">Todas las categorías</option>
        <option v-for="categoria in categorias" :key="categoria.id" :value="categoria.id">
          {{ categoria.nombre }}
        </option>
      </select>

      <label>
        <input
          v-model="vencidas"
          data-testid="filtro-vencidas"
          type="checkbox"
          @change="aplicarFiltros"
        />
        Solo vencidas
      </label>

      <input
        v-model="busqueda"
        data-testid="filtro-busqueda"
        placeholder="Buscar solicitudes"
        type="search"
      />

      <button data-testid="btn-limpiar-filtros" type="button" @click="limpiarFiltros">
        Limpiar filtros
      </button>
    </form>

    <p v-if="cargando" data-testid="listado-cargando">
      Cargando solicitudes...
    </p>

    <p v-else-if="error">
      {{ error }}
    </p>

    <p v-else-if="listado.items.length === 0" data-testid="listado-vacio">
      No hay solicitudes para mostrar.
    </p>

    <div v-else class="tabla-contenedor">
    <table data-testid="tabla-solicitudes">
      <thead>
        <tr>
          <th>Código</th>
          <th>Título</th>
          <th>Estado</th>
          <th>Prioridad</th>
          <th>SLA</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="solicitud in listado.items"
          :key="solicitud.id"
          data-testid="fila-solicitud"
          :data-codigo="solicitud.codigo"
        >
          <td data-testid="celda-codigo">
            <RouterLink :to="`/solicitudes/${solicitud.id}`">
              {{ solicitud.codigo }}
            </RouterLink>
          </td>
          <td>{{ solicitud.titulo }}</td>
          <td data-testid="celda-estado">{{ solicitud.estado }}</td>
          <td data-testid="celda-prioridad">{{ solicitud.prioridad }}</td>
          <td data-testid="celda-sla">
            {{ solicitud.fechaLimiteSla }}
            <span v-if="solicitud.vencida" data-testid="badge-vencida">
              Vencida
            </span>
          </td>
        </tr>
      </tbody>
    </table>
    </div>

    <section v-if="!cargando && !error">
      <button
        data-testid="paginacion-anterior"
        type="button"
        :disabled="listado.page <= 1"
        @click="paginaAnterior"
      >
        Anterior
      </button>

      <span data-testid="paginacion-info">
        Página {{ listado.page }} de {{ listado.totalPaginas }} — {{ listado.total }} resultados
      </span>

      <button
        data-testid="paginacion-siguiente"
        type="button"
        :disabled="listado.page >= listado.totalPaginas"
        @click="paginaSiguiente"
      >
        Siguiente
      </button>
    </section>
  </main>
</template>

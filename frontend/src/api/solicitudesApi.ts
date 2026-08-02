import { peticion } from "./clienteHttp"
import type {
  Categoria,
  DatosSolicitud,
  DatosTransicion,
  FiltrosSolicitudes,
  ListadoSolicitudes,
  SolicitudDetalle,
} from "../types/solicitudes"

export function obtenerCategorias() {
  return peticion<Categoria[]>("/categorias")
}

export function obtenerSolicitudes(filtros: FiltrosSolicitudes) {
  const parametros = new URLSearchParams({
    page: filtros.page.toString(),
    pageSize: filtros.pageSize.toString(),
  })

  if (filtros.estado) parametros.set("estado", filtros.estado)
  if (filtros.prioridad) parametros.set("prioridad", filtros.prioridad)
  if (filtros.categoriaId) parametros.set("categoriaId", filtros.categoriaId)
  if (filtros.agenteId) parametros.set("agenteId", filtros.agenteId)
  if (filtros.q) parametros.set("q", filtros.q)
  if (filtros.vencidas !== undefined) {
    parametros.set("vencidas", filtros.vencidas.toString())
  }
  if (filtros.sort) parametros.set("sort", filtros.sort)

  return peticion<ListadoSolicitudes>(`/solicitudes?${parametros.toString()}`)
}

export function crearSolicitud(datos: DatosSolicitud) {
  return peticion<SolicitudDetalle>("/solicitudes", {
    method: "POST",
    body: JSON.stringify(datos),
  })
}

export function obtenerSolicitud(id: string) {
  return peticion<SolicitudDetalle>(`/solicitudes/${id}`)
}

export function editarSolicitud(id: string, datos: DatosSolicitud) {
  return peticion<SolicitudDetalle>(`/solicitudes/${id}`, {
    method: "PUT",
    body: JSON.stringify(datos),
  })
}

export function ejecutarTransicion(id: string, datos: DatosTransicion) {
  return peticion<SolicitudDetalle>(`/solicitudes/${id}/transiciones`, {
    method: "POST",
    body: JSON.stringify(datos),
  })
}

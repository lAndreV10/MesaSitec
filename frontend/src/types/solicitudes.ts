export interface Categoria {
  id: string
  nombre: string
  slaHoras: number
}

export type EstadoSolicitud =
  | "Nueva"
  | "Asignada"
  | "EnProceso"
  | "Resuelta"
  | "Cerrada"
  | "Cancelada"

export type PrioridadSolicitud = "Baja" | "Media" | "Alta" | "Critica"

export interface UsuarioSolicitud {
  id: string
  nombre: string
}

export interface CategoriaSolicitud {
  id: string
  nombre: string
}

export interface SolicitudLista {
  id: string
  codigo: string
  titulo: string
  estado: EstadoSolicitud
  prioridad: PrioridadSolicitud
  categoria: CategoriaSolicitud
  agente: UsuarioSolicitud | null
  fechaCreacion: string
  fechaLimiteSla: string
  vencida: boolean
}

export interface ListadoSolicitudes {
  items: SolicitudLista[]
  page: number
  pageSize: number
  total: number
  totalPaginas: number
}

export interface SolicitudDetalle extends SolicitudLista {
  descripcion: string
  solicitante: UsuarioSolicitud
  fechaResolucion: string | null
  motivoResolucion: string | null
  motivoCancelacion: string | null
}

export interface DatosSolicitud {
  titulo: string
  descripcion: string
  categoriaId: string
  prioridad: PrioridadSolicitud
}

export type AccionTransicion =
  | "asignar"
  | "iniciar"
  | "resolver"
  | "cerrar"
  | "reabrir"
  | "cancelar"

export interface DatosTransicion {
  accion: AccionTransicion
  agenteId?: string
  motivo?: string
}

export interface FiltrosSolicitudes {
  estado?: EstadoSolicitud
  prioridad?: PrioridadSolicitud
  categoriaId?: string
  agenteId?: string
  q?: string
  vencidas?: boolean
  page: number
  pageSize: number
  sort?: "fechaCreacion" | "-fechaCreacion" | "prioridad" | "-prioridad" | "codigo"
}

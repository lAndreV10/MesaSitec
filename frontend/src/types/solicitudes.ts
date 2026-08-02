export interface Categoria {
  id: string
  nombre: string
  slaHoras: number
}

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
  estado: string
  prioridad: string
  categoria: CategoriaSolicitud
  agente: UsuarioSolicitud | null
  fechaCreacion: string
  fechaLimiteSla: string
  vencida: boolean
}

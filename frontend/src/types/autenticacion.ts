export interface InicioSesion{
    email: string
    password: string
}

export interface UsuarioSesion{
    id: string
    nombre: string
    email: string
    rol: "Admin" | "Agente" | "Solicitante"
    tenantId: string
    tenantNombre: string
}

export interface ResultadoInicioSesion{
    accessToken: string
    expiraEn: number
    usuario: UsuarioSesion 
}

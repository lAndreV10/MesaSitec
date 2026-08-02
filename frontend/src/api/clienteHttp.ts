export const URL_API = "http://localhost:5080/api/v1"
export async function peticion<T>(
  ruta: string,
  opciones: RequestInit = {},
): Promise<T> {
  const cabeceras = new Headers(opciones.headers)

  cabeceras.set("Content-Type", "application/json")

  const token = localStorage.getItem("accessToken")

  if (token) {
    cabeceras.set("Authorization", `Bearer ${token}`)
  }

  const respuesta = await fetch(`${URL_API}${ruta}`, {
    ...opciones,
    headers: cabeceras,
  })

  if (respuesta.status === 401) {
    localStorage.removeItem("accessToken")
    localStorage.removeItem("usuario")
    window.location.href = "/login"

    throw new Error("La sesión no es válida.")
  }

  if (!respuesta.ok) {
    const error = (await respuesta.json()) as { detail?: string }

    throw new Error(
      error.detail ?? "No fue posible completar la solicitud.",
    )
  }

  return respuesta.json() as Promise<T>
}

namespace Dominio;

public static class Estados
{
    public static string AplicarEstados(
        string estadoActual, 
        string accion)
    {
        return (estadoActual, accion) switch
        {
            ("Nueva", "asignar") => "Asignada",
            ("Nueva", "cancelar") => "Cancelada",
            ("Asignada", "iniciar") => "EnProceso",
            ("Asignada", "asignar") => "Asignada",
            ("Asignada", "cancelar") => "Cancelada",
            ("EnProceso", "resolver") => "Resuelta",
            ("EnProceso", "asignar") => "Asignada",
            ("EnProceso", "cancelar") => "Cancelada",
            ("Resuelta", "cerrar") => "Cerrada",
            ("Resuelta", "reabrir") => "EnProceso",
            _ => throw new InvalidOperationException(
                $"No se puede aplicar '{accion}' sobre una solicitud en estado '{estadoActual}'.")
        };
    }
}

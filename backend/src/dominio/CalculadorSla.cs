namespace Dominio;

public static class CalculadorSla
{
    public static DateTime CalcularFechaLimite(
        DateTime fechaCreacion,
        int slaHoras,
        string prioridad)
    {
        double factor = prioridad switch
        {
            "Critica" => 0.5,
            "Alta" => 0.75,
            "Media" => 1.0,
            "Baja" => 2.0,
            _ => throw new ArgumentException(
                "La prioridad no es válida",
                nameof(prioridad))
        };

        double horasCalculadas = slaHoras * factor;

        return fechaCreacion.AddHours(horasCalculadas);
    }
}
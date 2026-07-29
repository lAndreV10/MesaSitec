using Dominio;

namespace Dominio.Tests;

public class CalculadorSlaTests
{
    [Fact]
    public void CalcularFechaLimite_PrioridadCritica_ReduceSlaALaMitad()
    {
        DateTime fechaCreacion = new(
            2026, 1, 15, 8, 0, 0,
            DateTimeKind.Utc);

        DateTime resultado = CalculadorSla.CalcularFechaLimite(
            fechaCreacion,
            8,
            "Critica");

        DateTime fechaEsperada = new(
            2026, 1, 15, 12, 0, 0,
            DateTimeKind.Utc);

        Assert.Equal(fechaEsperada, resultado);
    }

    [Fact]
    public void CalcularFechaLimite_PrioridadAlta_AplicaFactorCeroSetentaYCinco()
    {
        DateTime fechaCreacion = new(
            2026, 1, 15, 8, 0, 0,
            DateTimeKind.Utc);

        DateTime resultado = CalculadorSla.CalcularFechaLimite(
            fechaCreacion,
            8,
            "Alta");

        DateTime fechaEsperada = new(
            2026, 1, 15, 14, 0, 0,
            DateTimeKind.Utc);

        Assert.Equal(fechaEsperada, resultado);
    }

    [Fact]
    public void CalcularFechaLimite_PrioridadMedia_MantieneSlaBase()
    {
        DateTime fechaCreacion = new(
            2026, 1, 15, 8, 0, 0,
            DateTimeKind.Utc);

        DateTime resultado = CalculadorSla.CalcularFechaLimite(
            fechaCreacion,
            8,
            "Media");

        DateTime fechaEsperada = new(
            2026, 1, 15, 16, 0, 0,
            DateTimeKind.Utc);

        Assert.Equal(fechaEsperada, resultado);
    }

    [Fact]
    public void CalcularFechaLimite_PrioridadBaja_DuplicaSlaBase()
    {
        DateTime fechaCreacion = new(
            2026, 1, 15, 8, 0, 0,
            DateTimeKind.Utc);

        DateTime resultado = CalculadorSla.CalcularFechaLimite(
            fechaCreacion,
            8,
            "Baja");

        DateTime fechaEsperada = new(
            2026, 1, 16, 0, 0, 0,
            DateTimeKind.Utc);

        Assert.Equal(fechaEsperada, resultado);
    }

    [Fact]
    public void CalcularFechaLimite_PrioridadInvalida_LanzaExcepcion()
    {
        DateTime fechaCreacion = new(
            2026, 1, 15, 8, 0, 0,
            DateTimeKind.Utc);

        Assert.Throws<ArgumentException>(() =>
            CalculadorSla.CalcularFechaLimite(
                fechaCreacion,
                8,
                "Urgente"));
    }
}
using Dominio;

namespace Dominio.Tests;
public class EstadosTests
{
    [Fact]
    public void AplicarEstados_NuevaAsignar_DevuelveAsignada()
    {
        string resultado = Estados.AplicarEstados(
            "Nueva",
            "asignar");
        Assert.Equal("Asignada", resultado);
    }

    [Fact]
    public void AplicarEstados_NuevaCancelar_DevuelveCancelada()
    {
        string resultado = Estados.AplicarEstados(
            "Nueva",
            "cancelar");
        Assert.Equal("Cancelada", resultado);
    }

    [Fact]
    public void AplicarEstados_AsignadaIniciar_DevuelveEnProceso()
    {
        string resultado = Estados.AplicarEstados(
            "Asignada",
            "iniciar");
        Assert.Equal("EnProceso", resultado);
    }

    [Fact]
    public void AplicarEstados_AsignadaAsignar_DevuelveAsignada()
    {
        string resultado = Estados.AplicarEstados(
            "Asignada",
            "asignar");
        Assert.Equal("Asignada", resultado);
    }

    [Fact]
    public void AplicarEstados_AsignadaCancelar_DevuelveCancelada()
    {
        string resultado = Estados.AplicarEstados(
            "Asignada",
            "cancelar");
        Assert.Equal("Cancelada", resultado);
    }

    
    [Fact]
    public void AplicarEstados_ResueltaCerrar_DevuelveCerrada()
    {
        string resultado = Estados.AplicarEstados(
            "Resuelta",
            "cerrar");
        Assert.Equal("Cerrada", resultado);
    }

    
    [Fact]
    public void AplicarEstados_ResueltaReabrir_DevuelveEnProceso()
    {
        string resultado = Estados.AplicarEstados(
            "Resuelta",
            "reabrir");
        Assert.Equal("EnProceso", resultado);
    }

    [Fact]
    public void AplicarEstados_EnProcesoResolver_DevuelveResuelta()
    {
        string resultado = Estados.AplicarEstados(
            "EnProceso",
            "resolver");
        Assert.Equal("Resuelta", resultado);
    }

    [Fact]
    public void AplicarEstados_EnProcesoAsignar_DevuelveAsignada()
    {
        string resultado = Estados.AplicarEstados(
            "EnProceso",
            "asignar");
        Assert.Equal("Asignada", resultado);
    }

    [Fact]
    public void AplicarEstados_EnProcesoCancelar_DevuelveCancelada()
    {
        string resultado = Estados.AplicarEstados(
            "EnProceso",
            "cancelar");
        Assert.Equal("Cancelada", resultado);
    }

}


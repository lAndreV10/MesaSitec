using Dominio;
namespace Dominio.Tests;

public class PermisosTests
{
    [Fact]
    public void PermisosRol_Admin_DevuelveTrue()
    {
        bool resultado = Permisos.PermisosRol("Admin");
        Assert.True(resultado);
    }
    [Fact]
    public void PermisosRol_Agente_DevuelveTrue()
    {
        bool resultado = Permisos.PermisosRol("Agente");
        Assert.True(resultado);
    }
    [Fact]
    public void PermisosRol_Solicitante_DevuelveFalse()
    {
        bool resultado = Permisos.PermisosRol("Solicitante");

        Assert.False(resultado);
    }

    [Fact]
    public void VerDetalle_AdminSolicitudAjena_DevuelveTrue()
    {
        bool resultado = Permisos.VerDetalle(
            "Admin",
            false);

        Assert.True(resultado);
    }

    [Fact]
    public void VerDetalle_AgenteSolicitudAjena_DevuelveTrue()
    {
        bool resultado = Permisos.VerDetalle(
            "Agente",
            false);

        Assert.True(resultado);
    }

    [Fact]
    public void VerDetalle_SolicitanteSolicitudPropia_DevuelveTrue()
    {
        bool resultado = Permisos.VerDetalle(
            "Solicitante",
            true);

        Assert.True(resultado);
    }

    [Fact]
    public void VerDetalle_SolicitanteSolicitudAjena_DevuelveFalse()
    {
        bool resultado = Permisos.VerDetalle(
            "Solicitante",
            false);

        Assert.False(resultado);
    }

    [Fact]
    public void PuedeCrear_Admin_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCrear("Admin");
        Assert.True(resultado);
    }

    [Fact]
    public void PuedeCrear_Agente_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCrear("Agente");
        Assert.True(resultado);
    }

    [Fact]
    public void PuedeCrear_Solicitante_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCrear("Solicitante");
        Assert.True(resultado);
    }

    [Fact]
    public void PuedeEditar_AdminSolicitudAjena_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeEditar(
            "Admin",
            false,
            "Cerrada");

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeEditar_AgenteSolicitudAjena_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeEditar(
            "Agente",
            false,
            "Resuelta");

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeEditar_SolicitantePropiaNueva_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeEditar(
            "Solicitante",
            true,
            "Nueva");

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeEditar_SolicitanteAjenaNueva_DevuelveFalse()
    {
        bool resultado = Permisos.PuedeEditar(
            "Solicitante",
            false,
            "Nueva");

        Assert.False(resultado);
    }

    [Fact]
    public void PuedeEditar_SolicitantePropiaAsignada_DevuelveFalse()
    {
        bool resultado = Permisos.PuedeEditar(
            "Solicitante",
            true,
            "Asignada");

        Assert.False(resultado);
    }

    [Fact]
    public void PuedeGestionar_Admin_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeGestionar("Admin");

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeGestionar_Agente_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeGestionar("Agente");

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeGestionar_Solicitante_DevuelveFalse()
    {
        bool resultado = Permisos.PuedeGestionar("Solicitante");

        Assert.False(resultado);
    }

    [Fact]
    public void PuedeCerrar_AdminSolicitudAjena_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCerrar(
            "Admin",
            false);

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeCerrar_AgenteSolicitudAjena_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCerrar(
            "Agente",
            false);

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeCerrar_SolicitanteSolicitudPropia_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCerrar(
            "Solicitante",
            true);

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeCerrar_SolicitanteSolicitudAjena_DevuelveFalse()
    {
        bool resultado = Permisos.PuedeCerrar(
            "Solicitante",
            false);

        Assert.False(resultado);
    }

    [Fact]
    public void PuedeCancelar_Admin_DevuelveTrue()
    {
        bool resultado = Permisos.PuedeCancelar("Admin");

        Assert.True(resultado);
    }

    [Fact]
    public void PuedeCancelar_Agente_DevuelveFalse()
    {
        bool resultado = Permisos.PuedeCancelar("Agente");

        Assert.False(resultado);
    }

    [Fact]
    public void PuedeCancelar_Solicitante_DevuelveFalse()
    {
        bool resultado = Permisos.PuedeCancelar("Solicitante");

        Assert.False(resultado);
    }
}

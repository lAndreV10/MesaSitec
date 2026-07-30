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
}

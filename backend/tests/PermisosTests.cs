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
}

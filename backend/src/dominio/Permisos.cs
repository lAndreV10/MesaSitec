namespace Dominio;
public static class Permisos
{
    public static bool PermisosRol(
        string rol)
    {
        return rol == "Admin"|| rol  == "Agente";
    }
}
